using Azure;
using Azure.AI.OpenAI;
using Claudia;
using OpenAI.ObjectModels.ResponseModels;
namespace Long2Long.L2L;

public record L2LPromptResult(string Output, string ErrorMessage)
{
    public static L2LPromptResult FromAnthropicMessageResult(MessageResponse messageResponse)
    {
        return new L2LPromptResult(
            messageResponse.Content.Aggregate(
                string.Empty,
                (s, content) => s + (content.Text ?? string.Empty)),
            messageResponse.StopReason);
    }
    public static L2LPromptResult FromGeminiMessageResult(
        string? generateContentResponse) =>
        new(
            generateContentResponse ?? string.Empty,
            string.Empty);
    public static L2LPromptResult FromAzureOpenAiMessageResult(
        Response<ChatCompletions>? completion)
    {
        if (completion is null)
        {
            return new L2LPromptResult(string.Empty, "Azure OpenAi からの応答がありません。");
        }
        return new L2LPromptResult(
            completion.Value.Choices.Aggregate(
                string.Empty,
                (s, choice) => s + choice.Message.Content),
            string.Empty);
    }
    public static L2LPromptResult FromOpenAiMessageResult(ChatCompletionCreateResponse message)
    {
        return new L2LPromptResult(
            message.Choices.Aggregate(
                string.Empty,
                (s, choice) => s + choice.Message.Content),
            string.Empty);
    }
}