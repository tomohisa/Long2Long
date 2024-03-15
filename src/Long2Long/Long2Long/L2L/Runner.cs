using Long2Long.Runners;
using System.Collections.Immutable;
namespace Long2Long.L2L;

public class Runner
{
    public static async Task<L2LResponse> RunAsync(L2LRequest request)
    {
        await Task.CompletedTask;
        var settings = request.Settings;
        var inputs = request.Inputs;
        var results = new List<L2LResults>();

        List<Task> tasks = new();
        if (settings.UseAnthropic)
        {
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var result = await AnthropicRunner.RunAsync(inputs, settings);
                        results.Add(result);
                    }));
        }
        if (settings.UseAzureOpenAi)
        {
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var result = await AzureOpenAiRunner.RunAsync(inputs, settings);
                        results.Add(result);
                    }));
        }
        if (settings.UseOpenAi)
        {
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var result = await OpenAiRunner.RunAsync(inputs, settings);
                        results.Add(result);
                    }));
        }
        if (settings.UseGemini)
        {
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        var result = await GeminiRunner.RunAsync(inputs, settings);
                        results.Add(result);
                    }));
        }
        await Task.WhenAll(tasks);
        // if (settings.UseAnthropic)
        // {
        //     var result = await AnthropicRunner.RunAsync(inputs, settings);
        //     results.Add(result);
        // }
        // if (settings.UseAzureOpenAi)
        // {
        //     var result = await AzureOpenAiRunner.RunAsync(inputs, settings);
        //     results.Add(result);
        // }
        // if (settings.UseOpenAi)
        // {
        //     var result = await OpenAiRunner.RunAsync(inputs, settings);
        //     results.Add(result);
        // }
        // if (settings.UseGemini)
        // {
        //     var result = await GeminiRunner.RunAsync(inputs, settings);
        //     results.Add(result);
        // }
        return new L2LResponse(results.ToImmutableList(), null);
    }
}
