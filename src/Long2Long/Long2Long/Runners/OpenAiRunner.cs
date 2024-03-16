using Long2Long.L2L;
using Long2Long.Settings;
using Long2Long.Texts;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
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
        if (string.IsNullOrWhiteSpace(settings.OpenAiApiKey))
        {
            return Task.FromResult(result with { ErrorMessage = "OpenAiApiKey が入力されていません。" });
        }
        if (string.IsNullOrWhiteSpace(settings.OpenAiModel))
        {
            return Task.FromResult(result with { ErrorMessage = "OpenAiModel が入力されていません。" });
        }
        var openAiClient = new OpenAIService(
            new OpenAiOptions
                { ApiKey = settings.OpenAiApiKey });

        foreach (var prompt in settings.Prompts)
        {
            try
            {
                var message = openAiClient.ChatCompletion.CreateCompletion(
                        new ChatCompletionCreateRequest
                        {
                            Messages = new List<ChatMessage>
                            {
                                ChatMessage.FromSystem(prompt.System),
                                ChatMessage.FromUser(prompt.User + text)
                            },
                            Model = settings.OpenAiModel
                        })
                    .Result;

                result = result with
                {
                    Phases = result.Phases.Add(
                        L2LPromptResult.FromOpenAiMessageResult(message))
                };
                text = message?.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
            }
            catch (Exception e)
            {
                result = result with
                {
                    Phases = result.Phases.Add(
                        new L2LPromptResult(string.Empty, e.Message))
                };
                Console.WriteLine(e);
            }
        }
        return Task.FromResult(result);
    }
}
