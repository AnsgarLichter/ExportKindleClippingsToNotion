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
    var client = new NotionClient(config.NotionAuthenticationToken, config.NotionDatabaseId);
    var exporter = new Exporter(client);

    //TODO: Check Stimmlers implementation
    var clippingsText = File.ReadAllText(pathToClippings);
    //TODO: Is \n and \n really necessary? Can I exclude these signs from the parsed file?
    var clippings = clippingsText.Split($"==========\r\n");
    //TODO: Use logger in real architecture
    Console.WriteLine($"Determined {clippings.Length} clippings");

    //TODO: Save Regex in a config object to get regex in dependence of config's language
    const string regexAuthor = "(?<=\\()(?!.+?\\()(.+?)(?=\\))";
    const string regexTitle = ".+?(?=\\s*\\()";
    const string regexPage = "(?<=Seite )[0-9]*";
    const string regexStartPosition = "[0-9]+(?=-)";
    const string regexFinishPosition = "(?<=-)[0-9]+";
    //TODO: Date Regex not working in every case
    const string regexDate = "\\d{2}[a-zA-Z_ .]*\\d{4}\\s*\\d{2}:\\d{2}:\\d{2}";
    const string regexClippingsLimitReached = "<.+?>";

    var books = new List<Book>();
    var parsedClippings = new List<Clipping>();
    foreach (var clipping in clippings)
    {
        var lines = clipping.Split("\n");
        if (lines.Length < 4)
        {
            Console.WriteLine("Found an invalid clipping. Parsing next one ...");
            continue;
        }

        var lineTitleAndAuthor = lines[0];
        var title = Regex.Match(lineTitleAndAuthor, regexTitle).Value;
        var author = Regex.Match(lineTitleAndAuthor, regexAuthor).Value;

        var linePagePositionDate = lines[1];
        var page = Regex.Match(linePagePositionDate, regexPage).Value;
        var startPosition = Regex.Match(linePagePositionDate, regexStartPosition).Value;
        var finishPosition = Regex.Match(linePagePositionDate, regexFinishPosition).Value;
        var date = Regex.Match(linePagePositionDate, regexDate).Value;

        var dateTime = DateTime.Parse(date, new CultureInfo(config.Language));

        var text = lines[3];
        if (Regex.IsMatch(text, regexClippingsLimitReached))
        {
            Console.WriteLine("Skipping clipping because limit has been reached");
            continue;
        }

        Console.WriteLine(
            $"{title} by {author}: Page {page} at position from {startPosition} to {finishPosition} created at {dateTime} - {text}");

        var parsedClipping = new Clipping(
            text,
            !string.IsNullOrEmpty(startPosition) ? int.Parse(startPosition) : 0,
            !string.IsNullOrEmpty(finishPosition) ? int.Parse(finishPosition) : 0,
            !string.IsNullOrEmpty(page) ? int.Parse(page) : 0,
            dateTime
        );

        var parsedBook = books.Find(x => x.Author == author && x.Title == title);
        if (parsedBook is null)
        {
            parsedBook = new Book(author, title);
            books.Add(parsedBook);
        }

        parsedClipping.Book = parsedBook;
        parsedBook.AddClipping(parsedClipping);

        parsedClippings.Add(parsedClipping);
    }

    Console.WriteLine($"Parsed {books.Count} books");
    Console.WriteLine($"Parsed {parsedClippings.Count} clippings");

    //TODO: Extract
    var booksService = new BooksService(new BaseClientService.Initializer());
    foreach (var book in books)
    {
        var volumes = await booksService.Volumes.List($"intitle:{book.Title}+inauthor:{book.Author}").ExecuteAsync();

        if (volumes.Items == null || volumes.Items.Count == 0)
        {
            //TODO: Use fallback image because Thumbnail is mandatory
            Console.WriteLine($"Use fallback cover for {book.Title}");
            continue;
        }

        var item = volumes.Items[0];
        book.Thumbnail = item.VolumeInfo.ImageLinks.Thumbnail;
        
        Console.WriteLine($"Use loaded cover for {book.Title}");
    }

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