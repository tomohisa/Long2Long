using System.Collections.Immutable;
namespace Long2Long.Texts;

public record SplitInputText(ImmutableList<ChunkedInputText> Chunks)
{
    public static SplitInputText Empty => new(ImmutableList<ChunkedInputText>.Empty);
    public static SplitInputText Create(InputText text, int chunkMaxSize, int chunkMinSize = 1) =>
        Create(LineSplitInputText.Create(text), chunkMaxSize, chunkMinSize);
    public static SplitInputText Create(
        LineSplitInputText text,
        int chunkMaxSize,
        int chunkMinSize = 1)
    {
        var target = Empty;

        var currentChunk = ChunkedInputText.Empty;

        foreach (var (line, i) in text.Lines.Select((inputText, i) => (inputText, i)))
        {
            var withNewLine = i == text.Lines.Count - 1 ? currentChunk.AppendWord(line)
                : currentChunk.AppendLine(line);
            if (withNewLine.CurrentTokenCount > chunkMaxSize)
            {
                if (currentChunk.CurrentTokenCount < chunkMinSize)
                {
                    var words = WordSplitInputText.Create(line);
                    foreach (var word in words.Words)
                    {
                        var withWord = currentChunk.AppendWord(word);
                        if (withWord.CurrentTokenCount > chunkMinSize)
                        {
                            target = target.Append(withWord);
                            currentChunk = ChunkedInputText.Empty;
                        }
                        else
                        {
                            currentChunk = withWord;
                        }
                    }
                }
                else
                {
                    target = target.Append(currentChunk);
                    currentChunk = ChunkedInputText.Empty.AppendLine(line);
                    if (currentChunk.CurrentTokenCount > chunkMaxSize)
                    {
                        currentChunk = ChunkedInputText.Empty;
                        var words = WordSplitInputText.Create(line);
                        foreach (var word in words.Words)
                        {
                            var withWord = currentChunk.AppendWord(word);
                            if (withWord.CurrentTokenCount > chunkMinSize)
                            {
                                target = target.Append(withWord);
                                currentChunk = ChunkedInputText.Empty;
                            }
                            else
                            {
                                currentChunk = withWord;
                            }
                        }
                    }
                }
            }
            else
            {
                currentChunk = withNewLine;
            }
        }
        if (currentChunk.CurrentTokenCount > 0)
        {
            target = target.Append(currentChunk);
        }
        return target;
    }
    public SplitInputText Append(ChunkedInputText chunk) =>
        new(Chunks.Add(chunk with { Id = Chunks.Count + 1 }));
    public int GetTotalLength() => Chunks.Sum(c => c.Text.Length);
}
