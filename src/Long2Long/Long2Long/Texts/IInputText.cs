namespace Long2Long.Texts;

public interface IInputText
{
    public static IInputText Create(string? text) => string.IsNullOrWhiteSpace(text)
        ? EmptyInputText.Instance : new InputText(text);
}
