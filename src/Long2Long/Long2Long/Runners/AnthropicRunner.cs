using Claudia;
using Long2Long.L2L;
using Long2Long.Settings;
using Long2Long.Texts;
using System.Collections.Immutable;
namespace Long2Long.Runners;

public static class AnthropicRunner
{
    public static async Task<L2LResults> RunAsync(SplitInputText inputs, Long2LongSettings settings)
    {
        var result = L2LResults.Default(L2LServiceProvider.Anthropic);

        var task = inputs.Chunks.Aggregate(
            ImmutableList<Task>.Empty,
            (current, chunk) => current.Add(
                Task.Run(
                    async () =>
                    {
                        var currentMessage = chunk.Text;
                        var chunkResult = await RunChunkAsync(chunk.Id, currentMessage, settings);
                        result = result.AppendChunk(chunkResult);
                    })));

        await Task.WhenAll(task);
        return result.OrderByChunkId();
    }

    public static Task<L2LChunkResult> RunChunkAsync(
        int id,
        string text,
        Long2LongSettings settings)
    {
        var result = L2LChunkResult.Empty with { Id = id };
        if (string.IsNullOrWhiteSpace(settings.AnthropicApiKey))
        {
            return Task.FromResult(result with { ErrorMessage = "APIキーが入力されていません。" });
        }
        var anthropic = new Anthropic
        {
            ApiKey = settings.AnthropicApiKey
        };

        foreach (var prompt in settings.Prompts)
        {
            try
            {
                var messageRequest = new MessageRequest
                {
                    Model = settings.AnthropicModel,
                    MaxTokens = 1024,
                    System = prompt.System,
                    Messages =
                    [
                        new Message
                        {
                            Role = "user",
                            Content = prompt.User + text
                        }
                    ]
                };
                var message = anthropic.Messages.CreateAsync(messageRequest).Result;
                result = result with
                {
                    Phases = result.Phases.Add(
                        L2LPromptResult.FromAnthropicMessageResult(message))
                };
                text = message.Content.LastOrDefault()?.Text ?? string.Empty;
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
