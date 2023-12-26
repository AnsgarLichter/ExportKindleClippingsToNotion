using EnumStringValues;

namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsLanguage()
{
    private const string EnglishClipping = "Your Highlight on page";
    private const string GermanClipping = "Ihre Markierung bei Position";
    private const string SpanishClipping = "La subrayado en la página";

    public SupportedLanguages Determine(string clipping)
    {
        var secondLine = clipping
            .Split("\r\n")
            .Where(line => line.Length > 0)
            .Select(line => line.Trim())
            .Select(line => line.Replace("\u200B", string.Empty))
            .ElementAtOrDefault(1);
        if (secondLine == null)
        {
            throw new LanguageNotRecognizedException("The language of the clipping couldn't be determined!");
        }
        else if (secondLine.Contains(EnglishClipping))
        {
            return SupportedLanguages.English;
        }
        else if (secondLine.Contains(GermanClipping))
        {
            return SupportedLanguages.German;
        }
        else if (secondLine.Contains(SpanishClipping))
        {
            throw new LanguageNotSupportedException(
                $"Spanish is currently not supported!");
        }

        throw new LanguageNotRecognizedException("The language of the clipping couldn't be determined!");
    }
}