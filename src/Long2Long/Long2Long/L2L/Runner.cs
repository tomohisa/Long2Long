using Long2Long.Runners;
using System.Collections.Immutable;
namespace Long2Long.L2L;

public class Runner
{
    public static async Task<L2LResponse> RunAsync(
        L2LRequest request,
        Action<int> reportTotal,
        Action<string, int> started,
        Action<string, int, string> ended)
    {
        await Task.CompletedTask;
        var settings = request.Settings;
        var inputs = request.Inputs;
        var results = new List<L2LResults>();

        var chunksCount = 0;

        List<Task> tasks = new();
        if (settings.UseAnthropic)
        {
            chunksCount += inputs.Chunks.Count;
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var anthropicRunner = new AnthropicRunner();
                        var result = await anthropicRunner.RunAsync(
                            inputs,
                            settings,
                            started,
                            ended);
                        results.Add(result);
                    }));
        }
        if (settings.UseAzureOpenAi)
        {
            chunksCount += inputs.Chunks.Count;
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var azure = new AzureOpenAiRunner();
                        var result = await azure.RunAsync(inputs, settings, started, ended);
                        results.Add(result);
                    }));
        }
        if (settings.UseOpenAi)
        {
            chunksCount += inputs.Chunks.Count;
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var openai = new OpenAiRunner();
                        var result = await openai.RunAsync(inputs, settings, started, ended);
                        results.Add(result);
                    }));
        }
        if (settings.UseGemini)
        {
            chunksCount += inputs.Chunks.Count;
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var gemini = new GeminiRunner();
                        var result = await gemini.RunAsync(inputs, settings, started, ended);
                        results.Add(result);
                    }));
        }
        reportTotal(chunksCount);
        await Task.WhenAll(tasks);

        return new L2LResponse(results.ToImmutableList(), null);
    }

    // generic method try to get L2LChunkResult and if error mssage is not empty, retry

    public static async Task<L2LChunkResult> RunChunkWithRetryAsync(
        int maxExecuteCount,
        Func<Task<L2LChunkResult>> runner)
    {
        var executeCount = 1;
        L2LChunkResult result;
        do
        {
            result = await runner();
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return result;
            }
            Console.WriteLine($"Error: {result.ErrorMessage} - retrying...");
            executeCount++;
        } while (executeCount <= maxExecuteCount);
        return result;
    }
}
