using Notion.Client;

namespace ExportKindleClippingsToNotion;

interface IImportClient
{
    Task<string[]> Import(string pathToClippings);
}

interface IImporter
{
    public Task<string[]> Import(string pathToClippings);
}

class Importer : IImporter
{
    private readonly IImportClient _client;

    public Importer(IImportClient client)
    {
        _client = client;
    }

    public Task<string[]>Import(string pathToClippings)
    {
        Console.WriteLine($"Starting import.");
        return _client.Import(pathToClippings);
    }
}