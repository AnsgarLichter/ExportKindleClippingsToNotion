namespace ExportKindleClippingsToNotion.Import;

public class FileClient : IImportClient
{

    public Task<string[]> Import(string pathToClippings)
    {
        return Task.FromResult(GetFormattedClippings(pathToClippings));
    }

    private static string[] GetFormattedClippings(string pathToClippings)
    {
        var clippingsText = File.ReadAllText(pathToClippings);
        return FormatClippings(clippingsText);
    }

    private static string[] FormatClippings(string clippingsText)
    {
        var clippings = clippingsText.Split($"==========\r\n");
        Console.WriteLine($"Determined {clippings.Length} clippings");

        return clippings;
    }
}