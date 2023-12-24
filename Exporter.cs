namespace ExportKindleClippingsToNotion;

interface IExporter
{
    Task Export(List<Book> books);
}

interface IExportClient
{
    Task Export(List<Book> books);
}

class Exporter : IExporter
{
    private readonly IExportClient _client;

    public Exporter(IExportClient client)
    {
        this._client = client;
    }

    public Task Export(List<Book> books)
    {
        Console.WriteLine($"Starting export.");
        return this._client.Export(books);
    }
}