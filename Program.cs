using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using ExportKindleClippingsToNotion;
using Notion.Client;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using NotionClient = ExportKindleClippingsToNotion.NotionClient;

//TODO: Implement logger instead of Console.WriteLine
if (args.Length == 0)
{
    Console.WriteLine("Please provide a path to your clippings file");
    return;
}

var pathToClippings = args[0];
if (!File.Exists(pathToClippings))
{
    Console.WriteLine($"Clippings File doesn't exist. Please check your path '{pathToClippings}'");
    return;
}

//TODO: Move into config class - validate while instantiation
//TODO: Handle no config found and all other exceptions
//TODO: Constant for path to config file
var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("params.json"));
if (config is null
    || string.IsNullOrEmpty(config.NotionAuthenticationToken)
    || string.IsNullOrEmpty(config.NotionDatabaseId)
    || string.IsNullOrEmpty(config.Language))
{
    Console.WriteLine(
        "Please provide your authentication token, your database id and your language in the config file");
    return;
}

try
{
    var importer = new Importer(new FileClient());
    var client = new NotionClient(config.NotionAuthenticationToken, config.NotionDatabaseId);
    var exporter = new Exporter(client);
    
    //TODO: Use logger in real architecture
    //TODO: Validate if importer is actually working
    var books = await importer.Import(pathToClippings);

    await exporter.Export(books);
}
catch (NotionApiException notionApiException)
{
    Console.WriteLine($"An error occurred communicating with notion: {notionApiException}");
}
catch (IOException ioException)
{
    Console.WriteLine($"An error occurred reading the clippings file: {ioException}");
}