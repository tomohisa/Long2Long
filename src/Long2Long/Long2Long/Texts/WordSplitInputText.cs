using System.Collections.Immutable;
namespace Long2Long.Texts;

public record WordSplitInputText(ImmutableList<InputText> Words)
{
    public static WordSplitInputText Create(InputText input)
    {
        var target = new WordSplitInputText(ImmutableList<InputText>.Empty);
        // split but not remove split char
        // foreach each char in text
        // if char is not space, add to current word
        // if char is space, add current word to words and reset current word
        var currentWord = InputText.Empty;
        foreach (var c in input.Text)
        {
            if ((((List<char>) [' ', '　', ',', '.', '。', '、']).Contains(c) &&
                    currentWord.Text.Length > 0) ||
                currentWord.GetLength() > 50)
            {
                target = target.Append(currentWord.Append(c));
                currentWord = InputText.Empty;
            }
            else
            {
                currentWord = currentWord.Append(c);
            }
        }
        return target;
    }
    public WordSplitInputText Append(InputText word) => new(Words.Add(word));
}
