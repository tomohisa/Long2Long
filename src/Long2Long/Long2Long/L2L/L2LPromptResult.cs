using Claudia;
using OpenAI.Chat;
namespace Long2Long.L2L;

public record L2LPromptResult(string Output, string ErrorMessage, int PromptId)
{
    public static L2LPromptResult FromAnthropicMessageResult(MessageResponse messageResponse, int promptId)
    {
        return new L2LPromptResult(
            messageResponse.Content.Aggregate(string.Empty, (s, content) => s + (content.Text ?? string.Empty)),
            messageResponse.StopReason,
            promptId);
    }
    public static L2LPromptResult FromGeminiMessageResult(string? generateContentResponse, int promptId) =>
        new(generateContentResponse ?? string.Empty, string.Empty, promptId);
    public static L2LPromptResult FromAzureOpenAiMessageResult(ChatCompletion generateContentResponse, int promptId) =>
        new(generateContentResponse.Content[0].Text ?? string.Empty, string.Empty, promptId);
}
