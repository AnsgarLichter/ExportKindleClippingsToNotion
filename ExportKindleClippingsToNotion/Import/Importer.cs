namespace ExportKindleClippingsToNotion.Import;

public interface IImportClient
{
    Task<string[]> Import(string pathToClippings);
}

public interface IImporter
{
    public Task<string[]> Import(string pathToClippings);
}

public class Importer(IImportClient client) : IImporter
{
    public Task<string[]>Import(string pathToClippings)
    {
        Console.WriteLine($"Starting import.");
        return client.Import(pathToClippings);
    }
}