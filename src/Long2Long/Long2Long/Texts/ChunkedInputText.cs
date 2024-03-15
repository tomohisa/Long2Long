namespace Long2Long.Texts;

public record ChunkedInputText(int Id, string Text)
{
    public static ChunkedInputText Empty => new(0, string.Empty);
    public int CurrentTokenCount => TokenCounter.ToTokenCount(Text);
    public ChunkedInputText AppendLine(InputText line) => new(Id, Text + line.Text + "\n");
    public ChunkedInputText AppendLine() => new(Id, Text + "\n");
    public ChunkedInputText AppendWord(InputText line) => new(Id, Text + line.Text);
}
