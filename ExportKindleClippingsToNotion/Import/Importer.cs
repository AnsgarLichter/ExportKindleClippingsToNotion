namespace ExportKindleClippingsToNotion.Import;

public interface IImportClient
{
    Task<string[]> ImportAsync(string pathToClippings);
}

public interface IImporter
{
    public Task<string[]> ImportAsync(string pathToClippings);
}

public class Importer(IImportClient client) : IImporter
{
    public async Task<string[]>ImportAsync(string pathToClippings)
    {
        Console.WriteLine($"Starting import.");
        return await client.ImportAsync(pathToClippings);
    }
}