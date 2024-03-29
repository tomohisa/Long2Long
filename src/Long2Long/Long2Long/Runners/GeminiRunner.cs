using GenerativeAI.Models;
using Long2Long.L2L;
using Long2Long.Settings;
using Long2Long.Texts;
namespace Long2Long.Runners;

public class GeminiRunner
{
    public async Task<L2LResults> RunAsync(
        SplitInputText inputs,
        Long2LongSettings settings,
        Action<string, int> started,
        Action<string, int, string> ended)
    {
        var result = L2LResults.Default(L2LServiceProvider.Gemini);

        foreach (var chunk in inputs.Chunks)
        {
            started(result.ServiceProvider.ToString(), chunk.Id);
            var currentMessage = chunk.Text;

            var chunkResult = await Runner.RunChunkWithRetryAsync(
                3,
                () => RunChunkAsync(chunk.Id, currentMessage, settings));

            result = result.AppendChunk(chunkResult);
            ended(result.ServiceProvider.ToString(), chunk.Id, chunkResult.ErrorMessage);
            await Task.Delay(2000);
        }
        return result.OrderByChunkId();
    }

    public Task<L2LChunkResult> RunChunkAsync(
        int id,
        string text,
        Long2LongSettings settings)
    {
        var result = L2LChunkResult.Empty with { Id = id };
        if (string.IsNullOrWhiteSpace(settings.GeminiApiKey))
        {
            return Task.FromResult(result with { ErrorMessage = "APIキーが入力されていません。" });
        }
        var gemini = new GenerativeModel(settings.GeminiApiKey, settings.GeminiModel);

        foreach (var prompt in settings.Prompts)
        {
            try
            {
                var response = gemini.GenerateContentAsync(prompt.System + prompt.User + text)
                    .Result;
                result = result with
                {
                    Phases = result.Phases.Add(
                        L2LPromptResult.FromGeminiMessageResult(response, prompt.Id))
                };
                text = response ?? string.Empty;
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
