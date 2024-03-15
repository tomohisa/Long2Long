using System.Collections.Immutable;
namespace Long2Long.L2L;

public record L2LResults(
    ImmutableList<L2LChunkResult> Results,
    L2LServiceProvider ServiceProvider,
    string ModelName,
    string ErrorMessage)
{
    public static L2LResults Default(L2LServiceProvider serviceProvider) => new(
        ImmutableList<L2LChunkResult>.Empty,
        serviceProvider,
        string.Empty,
        string.Empty);

    public L2LResults AppendChunk(L2LChunkResult chunk) =>
        this with { Results = Results.Add(chunk) };
    public L2LResults OrderByChunkId() =>
        this with { Results = Results.OrderBy(r => r.Id).ToImmutableList() };
    public string GetOutputText() => string.Join("\n", Results.Select(r => r.GetOutputText()));
}
