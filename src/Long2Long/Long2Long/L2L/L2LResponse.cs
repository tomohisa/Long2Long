using Long2Long.Settings;
using System.Collections.Immutable;
namespace Long2Long.L2L;

public record L2LResponse(ImmutableList<L2LResults> Results, string? Error)
{
    public void OutputWithFile(Long2LongSettings settings)
    {
        foreach (var result in Results)
        {
            foreach (var prompt in settings.Prompts)
            {
                var filename = settings.OutputFile;
                // add addition and period before the extension
                if (Results.Count > 1)
                {
                    var folder = Path.GetDirectoryName(filename);
                    filename = Path.GetFileNameWithoutExtension(filename) +
                        "." +
                        result.ServiceProvider +
                        Path.GetExtension(filename);
                    if (!string.IsNullOrWhiteSpace(folder))
                    {
                        filename = Path.Combine(folder, filename);
                    }
                }

                if (settings.Prompts.Count > 1)
                {
                    var folder = Path.GetDirectoryName(filename);
                    filename = Path.GetFileNameWithoutExtension(filename) +
                        ".P" +
                        prompt.Id +
                        Path.GetExtension(filename);
                    if (!string.IsNullOrWhiteSpace(folder))
                    {
                        filename = Path.Combine(folder, filename);
                    }
                }

                // write to file
                File.WriteAllText(filename, result.GetOutputText(prompt.Id));

            }

        }
    }
}
