namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsParserFactory
{
    public IClippingsParser GetByLanguage(SupportedLanguages languages)
    {
        return languages switch
        {
            SupportedLanguages.English => new ClippingsParserEnglish(),
            SupportedLanguages.German => new ClippingsParserGerman(),
            _ => throw new LanguageNotSupportedException("Your current language is not supported")
        };
    }
}