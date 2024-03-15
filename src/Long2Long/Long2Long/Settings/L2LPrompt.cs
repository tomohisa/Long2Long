namespace Long2Long.Settings;

public record L2LPrompt(string System, string User)
{
    public static L2LPrompt SimpleJapaneseToEnglish => new(
        "あなたは翻訳者です。文章を意訳せずにそのまま翻訳してください。",
        @"以下の文章を和訳してください。
------
");
}
