using System.Collections.Immutable;
namespace Long2Long.L2L;

public record L2LChunkResult(int Id, ImmutableList<L2LPromptResult> Phases, string ErrorMessage)
{
    public static L2LChunkResult Empty =>
        new(0, ImmutableList<L2LPromptResult>.Empty, string.Empty);

    public string GetOutputText(int promptId) =>
        Phases.LastOrDefault(m => m.PromptId == promptId)?.Output ?? string.Empty;
}
