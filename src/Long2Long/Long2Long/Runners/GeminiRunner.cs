using GenerativeAI.Models;
using Long2Long.L2L;
using Long2Long.Settings;
using Long2Long.Texts;
namespace Long2Long.Runners;

public static class GeminiRunner
{
    public static async Task<L2LResults> RunAsync(SplitInputText inputs, Long2LongSettings settings)
    {
        var result = L2LResults.Default(L2LServiceProvider.Gemini);

        foreach (var chunk in inputs.Chunks)
        {
            var currentMessage = chunk.Text;
            var chunkResult = await RunChunkAsync(chunk.Id, currentMessage, settings);
            result = result.AppendChunk(chunkResult);
            await Task.Delay(2000);
        }
        return result.OrderByChunkId();
    }

    public static Task<L2LChunkResult> RunChunkAsync(
        int id,
        string text,
        Long2LongSettings settings)
    {
        var result = L2LChunkResult.Empty with { Id = id };
        if (string.IsNullOrWhiteSpace(settings.GeminiApiKey))
        {
            return Task.FromResult(result with { ErrorMessage = "APIキーが入力されていません。" });
        }
        var gemini = new GenerativeModel(settings.GeminiApiKey);

        foreach (var prompt in settings.Prompts)
        {
            try
            {
                var response = gemini.GenerateContentAsync(prompt.System + prompt.User + text)
                    .Result;
                result = result with
                {
                    Phases = result.Phases.Add(
                        L2LPromptResult.FromGeminiMessageResult(response))
                };
                text = response ?? string.Empty;
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
