using AI.Dev.OpenAI.GPT;
namespace Long2Long.Texts;

public static class TokenCounter
{
    public static int ToTokenCount(string text)
    {
        var tokens = GPT3Tokenizer.Encode(text);
        return tokens.Count;
    }
}
