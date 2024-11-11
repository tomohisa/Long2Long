using Long2Long.L2L;
using Long2Long.Settings;
using Long2Long.Texts;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Immutable;
namespace Long2Long.Runners;

public class OpenAiRunner
{
    public SemaphoreSlim semaphore = new(3);
    public async Task<L2LResults> RunAsync(
        SplitInputText inputs,
        Long2LongSettings settings,
        Action<string, int> started,
        Action<string, int, string> ended)
    {
        var result = L2LResults.Default(L2LServiceProvider.OpenAi);

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

                            var chunkResult = await Runner.RunChunkWithRetryAsync(
                                3,
                                () => RunChunkAsync(chunk.Id, currentMessage, settings));

                            result = result.AppendChunk(chunkResult);
                            ended(result.ServiceProvider.ToString(), chunk.Id, chunkResult.ErrorMessage);
                        }
                        finally
                        {
                            await Task.Delay(1000);
                            semaphore.Release();
                        }
                    })));

        await Task.WhenAll(task);
        return result.OrderByChunkId();
    }

    public async Task<L2LChunkResult> RunChunkAsync(int id, string text, Long2LongSettings settings)
    {
        var result = L2LChunkResult.Empty with { Id = id };
        if (string.IsNullOrWhiteSpace(settings.OpenAiApiKey))
        {
            return await Task.FromResult(result with { ErrorMessage = "OpenAiApiKey が入力されていません。" });
        }
        if (string.IsNullOrWhiteSpace(settings.OpenAiModel))
        {
            return await Task.FromResult(result with { ErrorMessage = "OpenAiModel が入力されていません。" });
        }
        var clientOptions = new OpenAIClientOptions();
        var openAiClient = new OpenAIClient(new ApiKeyCredential(settings.OpenAiApiKey), clientOptions);


        foreach (var prompt in settings.Prompts)
        {
            try
            {
                var client = openAiClient.GetChatClient(settings.OpenAiModel);
                ChatCompletion completion = await client.CompleteChatAsync(
                    new SystemChatMessage(prompt.System),
                    new UserChatMessage(prompt.User + text));
                result = result with
                {
                    Phases = result.Phases.Add(L2LPromptResult.FromAzureOpenAiMessageResult(completion, prompt.Id))
                };
                text = completion?.Content[0].Text ?? string.Empty;
            }
            catch (Exception e)
            {
                result = result with
                {
                    Phases = result.Phases.Add(new L2LPromptResult(string.Empty, e.Message, prompt.Id)),
                    ErrorMessage = result.ErrorMessage + e.Message + " | "
                };
            }
        }
        return await Task.FromResult(result);
    }
}
