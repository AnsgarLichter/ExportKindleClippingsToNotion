namespace ExportKindleClippingsToNotion.Parser;

public interface IBooksParserFactory
{
    IBooksParser Create(IClippingsParser clippingsParser);
}