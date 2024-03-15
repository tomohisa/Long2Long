namespace Long2Long.Texts;

public record EmptyInputText : IInputText
{
    public static EmptyInputText Instance { get; } = new();
}
