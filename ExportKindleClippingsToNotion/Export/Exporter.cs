using ExportKindleClippingsToNotion.Model;

namespace ExportKindleClippingsToNotion.Export;

public interface IExporter
{
    Task ExportAsync(List<Book> books);
}

public interface IExportClient
{
    Task ExportAsync(List<Book> books);
}

public class Exporter(IExportClient client) : IExporter
{
    public async Task ExportAsync(List<Book> books)
    {
        Console.WriteLine($"Starting export.");
        await client.ExportAsync(books);
    }
}