using Long2Long.Settings;
using System.Collections.Immutable;
namespace Long2Long.L2L;

public record L2LResponse(ImmutableList<L2LResults> Results, string? Error)
{
    public void OutputWithFile(Long2LongSettings settings)
    {
        foreach (var result in Results)
        {
            var filename = settings.OutputFile;
            // add addition and period before the extension
            if (Results.Count > 1)
            {
                var extension = Path.GetExtension(filename);
                filename = Path.GetFileNameWithoutExtension(filename) +
                    "." +
                    result.ServiceProvider +
                    "." +
                    extension;
            }
            // write to file
            File.WriteAllText(filename, result.GetOutputText());
        }
    }
}
