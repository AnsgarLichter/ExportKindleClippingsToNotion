using Notion.Client;

namespace ExportKindleClippingsToNotion;

interface IImportClient
{
    Task<List<Book>> Import(string pathToClippings);
}

interface IImporter
{
    public Task<List<Book>> Import(string pathToClippings);
}

class Importer : IImporter
{
    private readonly IImportClient _client;

    public Importer(IImportClient client)
    {
        this._client = client;
    }

    public Task<List<Book>> Import(string pathToClippings)
    {
        Console.WriteLine($"Starting import.");
        return this._client.Import(pathToClippings);
    }
}