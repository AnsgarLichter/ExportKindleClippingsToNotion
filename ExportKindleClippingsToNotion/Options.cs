using CommandLine;

namespace ExportKindleClippingsToNotion;

public class Options
{

    [Option('p', "path", Required = true, HelpText = "Path to the clippings file")]
    public string PathToClippings { get; }

    [Option('a', "authenticationToken", Required = true,
        HelpText = "Your Notion Authentication Token to access the Notion API.")]
    public string NotionAuthenticationToken { get; }

    [Option('d', "databaseId", Required = true, HelpText = "Your Notion Database ID to export the clippings to.")]
    public string NotionDatabaseId { get; }
    
    public Options(string notionDatabaseId, string notionAuthenticationToken, string pathToClippings)
    {
        NotionDatabaseId = notionDatabaseId;
        NotionAuthenticationToken = notionAuthenticationToken;
        PathToClippings = pathToClippings;
    }
}