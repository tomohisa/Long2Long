using Long2Long.Settings;
using Long2Long.Texts;
namespace Long2Long.L2L;

public record L2LRequest(
    SplitInputText Inputs,
    Long2LongSettings Settings) : IL2LPreprocessor
{
}
