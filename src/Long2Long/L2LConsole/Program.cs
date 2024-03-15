using Long2Long.L2L;
using Long2Long.Settings;
using System.Diagnostics;

// 設定ファイルの取得、デフォルト以外の場合は、JSONファイル名をコマンドライン引数1つ目で指定する
// 例: dotnet run settings.json
var settings = Long2LongSettings.FromCommandLine(args);

var preprocess = IL2LPreprocessor.PreprocessAsync(settings);

switch (preprocess)
{
    case L2LPreprocessorFailed failed:
        Console.WriteLine(failed.Error);
        return;
    case L2LRequest request:
        Console.WriteLine(request.Inputs.Chunks.Count + "個のチャンクに分割しました");

        // RunAsync にかかった時間を計算して表示したい
        var sw = new Stopwatch();
        sw.Start();
        Console.WriteLine("作業開始: " + DateTime.Now.ToString("h:mm:ss tt"));

        var results = await Runner.RunAsync(request);
        sw.Stop();
        Console.WriteLine("作業終了: " + DateTime.Now.ToString("h:mm:ss tt"));
        Console.WriteLine("処理時間: " + sw.Elapsed + "秒");
        results.OutputWithFile(settings);
        break;
}
