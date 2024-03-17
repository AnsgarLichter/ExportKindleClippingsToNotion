namespace ExportKindleClippingsToNotion.Parser;

public interface IClippingsParserFactory
{
    IClippingsParser GetByLanguage(SupportedLanguages languages);
}