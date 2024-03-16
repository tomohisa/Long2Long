using Azure;
using Azure.AI.OpenAI;
using Claudia;
using OpenAI.ObjectModels.ResponseModels;
namespace Long2Long.L2L;

public record L2LPromptResult(string Output, string ErrorMessage, int PromptId)
{
    public static L2LPromptResult FromAnthropicMessageResult(
        MessageResponse messageResponse,
        int promptId)
    {
        return new L2LPromptResult(
            messageResponse.Content.Aggregate(
                string.Empty,
                (s, content) => s + (content.Text ?? string.Empty)),
            messageResponse.StopReason,
            promptId);
    }
    public static L2LPromptResult FromGeminiMessageResult(
        string? generateContentResponse,
        int promptId) =>
        new(
            generateContentResponse ?? string.Empty,
            string.Empty,
            promptId);
    public static L2LPromptResult FromAzureOpenAiMessageResult(
        Response<ChatCompletions>? completion,
        int promptId)
    {
        if (completion is null)
        {
            return new L2LPromptResult(string.Empty, "Azure OpenAi からの応答がありません。", promptId);
        }
        return new L2LPromptResult(
            completion.Value.Choices.Aggregate(
                string.Empty,
                (s, choice) => s + choice.Message.Content),
            string.Empty,
            promptId);
    }
    public static L2LPromptResult FromOpenAiMessageResult(
        ChatCompletionCreateResponse message,
        int promptId)
    {
        return new L2LPromptResult(
            message.Choices.Aggregate(
                string.Empty,
                (s, choice) => s + choice.Message.Content),
            string.Empty,
            promptId);
    }
}
