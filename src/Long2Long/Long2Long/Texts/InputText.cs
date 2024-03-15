namespace Long2Long.Texts;

public record InputText(string Text) : IInputText
{
    public static InputText Empty => new(string.Empty);
    public int GetLength() => Text.Length;
    public int GetLineCount() => Text.Split('\n').Length;
    public InputText Append(char character) => new(Text + character);
}
