namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsParserFactory
{
    public IClippingsParser GetByLanguage(SupportedLanguages languages)
    {
        return languages switch
        {
            SupportedLanguages.English => new ClippingsParser(ClippingsLanguageConfigurations.English),
            SupportedLanguages.German => new ClippingsParser(ClippingsLanguageConfigurations.German),
            _ => throw new LanguageNotSupportedException("Your current language is not supported")
        };
    }
}