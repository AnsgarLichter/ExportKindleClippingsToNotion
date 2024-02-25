using System.IO.Abstractions;

namespace ExportKindleClippingsToNotion.Import;

public class FileClient(IFileSystem fileSystem) : IImportClient
{
    public async Task<string[]> Import(string pathToClippings)
    {
        return await GetFormattedClippings(pathToClippings);
    }

    private async Task<string[]> GetFormattedClippings(string pathToClippings)
    {
        var clippingsText = await fileSystem.File.ReadAllTextAsync(pathToClippings);
        return FormatClippings(clippingsText);
    }

    private string[] FormatClippings(string clippingsText)
    {
        var clippings = clippingsText.Split($"==========\r\n");
        Console.WriteLine($"Determined {clippings.Length} clippings");

        return clippings;
    }
}