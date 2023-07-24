namespace ExportKindleClippingsToNotion;

internal interface IExporter
{
    void Export(List<Book> books);
}

interface IExportClient
{
    void Export(List<Book> books);
}

internal class Exporter : IExporter
{
    private readonly IExportClient _client;

    public Exporter(IExportClient client)
    {
        this._client = client;
    }

    public void Export(List<Book> books)
    {
        Console.WriteLine($"Starting export.");
        this._client.Export(books);
    }
}