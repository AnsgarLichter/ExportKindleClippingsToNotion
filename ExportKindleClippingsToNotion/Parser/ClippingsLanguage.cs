using static System.String;

namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsLanguage() : IClippingsLanguage
{
    private const string EnglishClipping = "Your Highlight";
    private const string GermanClipping = "Ihre Markierung";
    private const string RussianClipping = "Ваш";
    private const string SpanishClipping = "La subrayado";

    private readonly Dictionary<SupportedLanguages, string> _languageIdentifiers =
        new()
        {
            { SupportedLanguages.English, EnglishClipping },
            { SupportedLanguages.German, GermanClipping },
            { SupportedLanguages.Russian, RussianClipping }
        };

    public SupportedLanguages Determine(string clipping)
    {
        var secondLine = clipping
            .Split("\n")
            .Where(line => line.Length > 0)
            .Select(line => line.Trim())
            .Select(line => line.Replace("\u200B", Empty))
            .ElementAtOrDefault(1);

        if (IsNullOrWhiteSpace(secondLine?.Trim()))
        {
            throw new LanguageNotRecognizedException("The language of your clipping can't be recognized!");
        }
        
        foreach (var identifier in _languageIdentifiers
                     .Where(identifier => secondLine.Contains(identifier.Value)))
            return identifier.Key;

        throw new LanguageNotSupportedException("The language of your clipping is currently not supported!");
    }
}