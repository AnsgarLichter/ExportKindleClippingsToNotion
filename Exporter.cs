interface IExporter
{
    void export(List<Book> books);
}

interface IExportClient
{
    void export(List<Book> books);
}

class Exporter : IExporter
{
    private readonly IExportClient client;

    public Exporter(IExportClient client)
    {
        this.client = client;
    }

    public void export(List<Book> books)
    {
        Console.WriteLine($"Starting export.");
        this.client.export(books);
    }
}