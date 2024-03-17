using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Parser;

namespace ExportKindleClippingsToNotion;

public class ExportKindleClippingsToNotion(
    IImporter importer,
    IBooksParserFactory booksParserFactory,
    IExporter exporter,
    IClippingsLanguage clippingsLanguage,
    IClippingsParserFactory clippingsParserFactory)
{
    public async Task ExecuteAsync(string pathToClippings)
    {
        var clippings = await importer.ImportAsync(pathToClippings);
        if (clippings.Length == 0)
        {
            Console.WriteLine("No clippings found");
            return;
        }

        var language = clippingsLanguage.Determine(clippings[0]);
        var clippingsParser = clippingsParserFactory.GetByLanguage(language);
        var booksParser = booksParserFactory.Create(clippingsParser);
        var books = await booksParser.ParseAsync(clippings);
        await exporter.ExportAsync(books);
    }
}