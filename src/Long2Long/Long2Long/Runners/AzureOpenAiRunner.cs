using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Long2Long.L2L;
using Long2Long.Settings;
using Long2Long.Texts;
using System.Collections.Immutable;
namespace Long2Long.Runners;

public class AzureOpenAiRunner
{
    public SemaphoreSlim semaphore = new(3);
    public async Task<L2LResults> RunAsync(
        SplitInputText inputs,
        Long2LongSettings settings,
        Action<string, int> started,
        Action<string, int, string> ended)
    {
        var result = L2LResults.Default(L2LServiceProvider.AzureOpenAi);

        var task = inputs.Chunks.Aggregate(
            ImmutableList<Task>.Empty,
            (current, chunk) => current.Add(
                Task.Run(
                    async () =>
                    {
                        try
                        {
                            await semaphore.WaitAsync();
                            started(result.ServiceProvider.ToString(), chunk.Id);
                            var currentMessage = chunk.Text;
                            var chunkResult = await RunChunkAsync(
                                chunk.Id,
                                currentMessage,
                                settings);
                            result = result.AppendChunk(chunkResult);
                            ended(
                                result.ServiceProvider.ToString(),
                                chunk.Id,
                                chunkResult.ErrorMessage);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    })));

        await Task.WhenAll(task);
        return result.OrderByChunkId();
    }

    public Task<L2LChunkResult> RunChunkAsync(
        int id,
        string text,
        Long2LongSettings settings)
    {
        var result = L2LChunkResult.Empty with { Id = id };
        if (string.IsNullOrWhiteSpace(settings.AzureOpenAiUrl))
        {
            return Task.FromResult(result with { ErrorMessage = "AzureOpenAiUrl が入力されていません。" });
        }
        if (string.IsNullOrWhiteSpace(settings.AzureOpenAiApiKey))
        {
            return Task.FromResult(result with { ErrorMessage = "AzureOpenAiApiKey が入力されていません。" });
        }
        if (string.IsNullOrWhiteSpace(settings.AzureOpenAiModel))
        {
            return Task.FromResult(result with { ErrorMessage = "AzureOpenAiModel が入力されていません。" });
        }
        var clientOptions = new OpenAIClientOptions
        {
            Retry =
            {
                NetworkTimeout = TimeSpan.FromMinutes(5), Mode = RetryMode.Exponential,
                MaxRetries = 2
            }
        };
        var openAiClient = new OpenAIClient(
            new Uri(settings.AzureOpenAiUrl),
            new AzureKeyCredential(settings.AzureOpenAiApiKey),
            clientOptions);

        foreach (var prompt in settings.Prompts)
        {
            try
            {
                var options = new ChatCompletionsOptions();
                options.DeploymentName = settings.AzureOpenAiModel;
                options.Messages.Add(new ChatRequestSystemMessage(prompt.System));
                options.Messages.Add(new ChatRequestUserMessage(prompt.User + text));

                var message = openAiClient.GetChatCompletions(options);
                // get all stream
                result = result with
                {
                    Phases = result.Phases.Add(
                        L2LPromptResult.FromAzureOpenAiMessageResult(message, prompt.Id))
                };
                text = message?.Value.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
            }
            catch (Exception e)
            {
                result = result with
                {
                    Phases = result.Phases.Add(
                        new L2LPromptResult(string.Empty, e.Message, prompt.Id)),
                    ErrorMessage = result.ErrorMessage + e.Message + " | "
                };
            }
        }
        return Task.FromResult(result);
    }
}
