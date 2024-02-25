using ExportKindleClippingsToNotion.Config;
using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Parser;

namespace ExportKindleClippingsToNotion;

public class ExportKindleClippingsToNotion(IImporter importer, IBooksParser booksParser, IExporter exporter)
{
    public async Task ExecuteAsync(string pathToClippings)
    {
        var clippings = await importer.Import(pathToClippings);
        var books = await booksParser.ParseAsync(clippings);
        await exporter.Export(books);
    }
}