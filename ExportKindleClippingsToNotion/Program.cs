using ExportKindleClippingsToNotion.Config;
using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Parser;
using Notion.Client;
using NotionClient = ExportKindleClippingsToNotion.Notion.NotionClient;

const string pathToConfig = "../params.json";
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
    var clippingsParser = new ClippingsParser(ClippingsLanguageConfigurations.German);

    var importer = new Importer(new FileClient());
    var booksParser = new BooksParser(metadataFetcher, clippingsParser);
    var exporter = new Exporter(client);
    
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