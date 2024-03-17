using ExportKindleClippingsToNotion.Import.Metadata;

namespace ExportKindleClippingsToNotion.Parser;

public class BooksParserFactory(IBookMetadataFetcher metadataFetcher) : IBooksParserFactory
{
    public IBooksParser Create(IClippingsParser clippingsParser)
    {
        return new BooksParser(metadataFetcher, clippingsParser);
    }
}