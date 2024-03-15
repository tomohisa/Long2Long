using System.Collections.Immutable;
using System.Text.Json;
namespace Long2Long.Settings;

public record Long2LongSettings
{
    public bool UseConsoleMessages { get; init; } = true;

    /// <summary>
    ///     if null, use standard input
    /// </summary>
    public string? InputFile { get; init; }

    /// <summary>
    ///     if null, use "output.txt"
    /// </summary>
    public string OutputFile { get; init; } = "output.txt";

    public bool OutputByChunk { get; init; }
    public bool OutputAllTogether { get; init; } = true;

    public int MaxTokenCount { get; init; } = 5000;
    public int MinTokenCount { get; init; } = 4500;

    public bool UseAnthropic { get; init; }
    public string AnthropicModel { get; init; } = "claude-3-opus-20240229";
    /// <summary>
    ///     if empty, use environment variable ANTHROPIC_API_KEY
    /// </summary>
    public string AnthropicApiKey { get; init; } = string.Empty;

    public bool UseAzureOpenAi { get; init; }
    /// <summary>
    ///     if empty, use environment variable AZURE_OPENAI_API_URL
    /// </summary>
    public string? AzureOpenAiUrl { get; init; }
    /// <summary>
    ///     if empty, use environment variable AZURE_OPENAI_API_MODEL
    /// </summary>
    public string? AzureOpenAiModel { get; init; }
    /// <summary>
    ///     if empty, use environment variable AZURE_OPENAI_API_KEY
    /// </summary>
    public string AzureOpenAiApiKey { get; init; } = string.Empty;

    public bool UseOpenAi { get; init; }
    public string OpenAiModel { get; init; } = "gpt-4-turbo-preview";
    /// <summary>
    ///     if empty, use environment variable OPENAI_API_KEY
    /// </summary>
    public string OpenAiApiKey { get; init; } = string.Empty;

    public bool UseGemini { get; init; }
    public string GeminiModel { get; init; } = "gemini-pro";
    /// <summary>
    ///     if empty, use environment variable GEMINI_API_KEY
    /// </summary>
    public string GeminiApiKey { get; init; } = string.Empty;

    public L2LPrompt? Prompt { get; init; }

    public ImmutableList<L2LPrompt> Prompts { get; init; } = [];

    public static Long2LongSettings Default { get; } = new();
    public static Long2LongSettings FromCommandLine(string[] args)
    {
        var settings = new Long2LongSettings();
        var arguments = args;
        if (args.Length == 1)
        {
            var filename = args.First();
            if (!filename.EndsWith(".json"))
            {
                filename += ".json";
            }
            settings = JsonSerializer.Deserialize<Long2LongSettings>(File.ReadAllText(filename)) ??
                settings;
            return settings.MergePromptWithPrompts().FillFromEnvironment();
        }
        Console.WriteLine("コマンドラインのパラメーターで設定ファイルを指定してください。");
        Console.WriteLine("例: L2L settings.json");

        return settings.MergePromptWithPrompts().FillFromEnvironment();
    }
    public Long2LongSettings MergePromptWithPrompts() => this with
    {
        Prompts = Prompt is not null ? Prompts.Add(Prompt) : Prompts, Prompt = null
    };
    public Long2LongSettings FillFromEnvironment()
    {
        var settings = this;
        if (UseAnthropic && string.IsNullOrWhiteSpace(AnthropicApiKey))
        {
            settings = settings with
            {
                AnthropicApiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
                string.Empty
            };
        }
        if (UseAzureOpenAi && string.IsNullOrWhiteSpace(AzureOpenAiUrl))
        {
            settings = settings with
            {
                AzureOpenAiUrl = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_URL") ??
                string.Empty
            };
        }
        if (UseAzureOpenAi && string.IsNullOrWhiteSpace(AzureOpenAiApiKey))
        {
            settings = settings with
            {
                AzureOpenAiApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ??
                string.Empty
            };
        }
        if (UseAzureOpenAi && string.IsNullOrWhiteSpace(AzureOpenAiModel))
        {
            settings = settings with
            {
                AzureOpenAiModel = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_MODEL") ??
                string.Empty
            };
        }
        if (UseOpenAi && string.IsNullOrWhiteSpace(OpenAiApiKey))
        {
            settings = settings with
            {
                OpenAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                string.Empty
            };
        }
        if (UseGemini && string.IsNullOrWhiteSpace(GeminiApiKey))
        {
            settings = settings with
            {
                GeminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                string.Empty
            };
        }
        return settings;
    }
}
