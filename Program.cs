using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using ExportKindleClippingsToNotion;
using Notion.Client;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using NotionClient = ExportKindleClippingsToNotion.NotionClient;

const string pathToConfig = "params.json";
//TODO: Use logger in real architecture that writes the log to the console
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

try
{
    var config = new Config(pathToConfig);

    var client = new NotionClient(config.NotionAuthenticationToken, config.NotionDatabaseId);
    var metadataFetcher = new GoogleBooksClient();
    var clippingsParser = new ClippingsParser();

    var importer = new Importer(new FileClient());
    var booksParser = new BooksParser(metadataFetcher, clippingsParser);
    var exporter = new Exporter(client);

    //TODO: Validate if importer & parser are actually working
    var clippings = await importer.Import(pathToClippings);
    var books = await booksParser.Parse(clippings);
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

catch (Exception exception)
{
    Console.WriteLine($"An error occurred: {exception}");
}