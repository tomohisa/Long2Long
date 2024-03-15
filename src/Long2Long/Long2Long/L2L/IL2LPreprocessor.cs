using Long2Long.Settings;
using Long2Long.Texts;
namespace Long2Long.L2L;

public interface IL2LPreprocessor
{
    public static IL2LPreprocessor PreprocessAsync(Long2LongSettings settings)
    {
        if (settings.UseConsoleMessages)
        {
            Console.WriteLine(
                "入力コンテンツ取得中です..." +
                (string.IsNullOrEmpty(settings.InputFile)
                    ? "入力ファイル未指定なので、質問のみを実行します。"
                    : $"ファイル:{settings.InputFile} から読み込んでいます。"));
        }
        var all = string.IsNullOrEmpty(settings.InputFile) ? " _ "
            : File.ReadAllText(settings.InputFile);
        var inputText = IInputText.Create(all);
        if (inputText is not InputText input)
        {
            return new L2LPreprocessorFailed($"入力ファイル{settings.InputFile}が空です");
        }
        return new L2LRequest(
            SplitInputText.Create(input, settings.MaxTokenCount, settings.MinTokenCount),
            settings);
    }
}
