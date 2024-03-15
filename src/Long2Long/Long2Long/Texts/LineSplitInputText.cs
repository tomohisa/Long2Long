using System.Collections.Immutable;
namespace Long2Long.Texts;

public record LineSplitInputText(ImmutableList<InputText> Lines)
{
    public static LineSplitInputText Create(InputText input)
    {
        var lines = ImmutableList.CreateBuilder<InputText>();
        foreach (var line in input.Text.Split('\n'))
        {
            lines.Add(new InputText(line));
        }
        return new LineSplitInputText(lines.ToImmutable());
    }
}
