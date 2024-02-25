using System.IO.Abstractions;
using ExportKindleClippingsToNotion.Config;
using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Notion.Utils;
using ExportKindleClippingsToNotion.Parser;
using Notion.Client;
using BooksService = ExportKindleClippingsToNotion.Import.Metadata.BooksService;
using NotionClient = ExportKindleClippingsToNotion.Notion.NotionClient;

const string pathToConfig = "../params.json";
if (args.Length == 0)
{
    Console.WriteLine("Please provide a path to your clippings file");
    return;
}

try
{
    var fileSystem = new FileSystem();
    var configReader = new ConfigReader(fileSystem);
    var config = await configReader.ExecuteAsync(pathToConfig);
    
    var notionClient = NotionClientFactory.Create(
        new ClientOptions
        {
            AuthToken = config.NotionAuthenticationToken
        }
    );
    var client = new NotionClient(config.NotionDatabaseId, notionClient, new PagesUpdateParametersBuilder());
    var importer = new Importer(new FileClient(fileSystem));
    var exporter = new Exporter(client);
    
    var metadataFetcher = new GoogleBooksClient(new BooksService());
    var clippingsParser = new ClippingsParserGerman();
    var booksParser = new BooksParser(metadataFetcher, clippingsParser);
    
    var exportKindleClippingsToNotion = new ExportKindleClippingsToNotion.ExportKindleClippingsToNotion(importer, booksParser, exporter);
    var pathToClippings = args[0];
    await exportKindleClippingsToNotion.ExecuteAsync(pathToClippings);
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