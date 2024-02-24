using ExportKindleClippingsToNotion.Model;

namespace ExportKindleClippingsToNotion.Export;

public interface IExporter
{
    Task Export(List<Book> books);
}

public interface IExportClient
{
    Task Export(List<Book> books);
}

public class Exporter(IExportClient client) : IExporter
{
    public Task Export(List<Book> books)
    {
        Console.WriteLine($"Starting export.");
        return client.Export(books);
    }
}