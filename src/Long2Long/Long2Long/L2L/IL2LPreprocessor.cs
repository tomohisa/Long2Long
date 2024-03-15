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
                    ? "標準入力から読み込んでいます。ctrl + z で終了できます。(macOS: ctrl + c)"
                    : $"ファイル:{settings.InputFile} から読み込んでいます。"));
        }
        var all = string.IsNullOrEmpty(settings.InputFile) ? Console.In.ReadToEnd()
            : File.ReadAllText(settings.InputFile);
        var inputText = IInputText.Create(all);
        if (inputText is not InputText input)
        {
            return new L2LPreprocessorFailed("入力が空です");
        }
        return new L2LRequest(
            SplitInputText.Create(input, settings.MaxTokenCount, settings.MinTokenCount),
            settings);
    }
}
