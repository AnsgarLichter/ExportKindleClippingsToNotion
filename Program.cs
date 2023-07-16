using System.Text.Json;
using System.Text.RegularExpressions;

using System.Globalization;

using Notion.Client;
using Google.Apis.Books.v1;
using Google.Apis.Services;

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
Config? config = JsonSerializer.Deserialize<Config>(File.ReadAllText("./params.json"));
if (config is null
    || string.IsNullOrEmpty(config.NotionAuthenticationToken)
    || string.IsNullOrEmpty(config.NotionDatabaseId)
    || string.IsNullOrEmpty(config.Language))
{
    Console.WriteLine("Please provide your authentication token, your database id and your language in the config file");
    return;
}

try
{
    var client = new NotionClient(config.NotionAuthenticationToken, config.NotionDatabaseId);
    var exporter = new Exporter(client);

    //TODO: Check Stimmlers implementation
    String clippingsText = File.ReadAllText(pathToClippings);
    //TODO: Is \n and \n really necessary? Can I exclude these signs from the parsed file?
    String[] clippings = clippingsText.Split($"==========\r\n");
    //TODO: Use logger in real architecture
    Console.WriteLine($"Determined {clippings.Length} clippings");

    //TODO: Save Regex in a config object to get regex in dependence of config's language
    var regexAuthor = "(?<=\\()(?!.+?\\()(.+?)(?=\\))";
    var regexTitle = ".+?(?=\\s*\\()";
    var regexPage = "(?<=Seite )[0-9]*";
    var regexStartPosition = "[0-9]+(?=-)";
    var regexFinishPosition = "(?<=-)[0-9]+";
    var regexDate = "\\d{2}[a-zA-Z_ .]*\\d{4}\\s*\\d{2}:\\d{2}:\\d{2}";
    var regexClippingsLimitReached = "<.+?>";

    List<Book> books = new List<Book>();
    List<Clipping> parsedClippings = new List<Clipping>();
    foreach (var clipping in clippings)
    {
        var lines = clipping.Split("\n");
        if (lines.Length < 4)
        {
            Console.WriteLine("Found an invalid clipping. Parsing next one ...");
            continue;
        }

        var lineTilteAndAuthor = lines[0];
        var title = Regex.Match(lineTilteAndAuthor, regexTitle).Value;
        var author = Regex.Match(lineTilteAndAuthor, regexAuthor).Value;

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

        Console.WriteLine($"{title} by {author}: Page {page} at position from {startPosition} to {finishPosition} created at {dateTime} - {text}");

        Clipping parsedClipping = new Clipping(
            text,
            !string.IsNullOrEmpty(startPosition) ? Int32.Parse(startPosition) : 0,
            !string.IsNullOrEmpty(finishPosition) ? Int32.Parse(finishPosition) : 0,
            !string.IsNullOrEmpty(page) ? Int32.Parse(page) : 0,
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


    var httpClient = new HttpClient()
    {
        BaseAddress = new Uri("https://www.googleapis.com/books/v1/volumes") //TODO: Move into config
    };

    //TODO: Extract
    var booksService = new BooksService(new BaseClientService.Initializer());
    foreach (var book in books)
    {
        var volumes = await booksService.Volumes.List($"intitle:{book.Title}+inauthor:{book.Author}").ExecuteAsync();

        if (volumes.Items == null || volumes.Items.Count == 0)
        {
            //TODO: Use fallback image because Thumbnail is mandatory
            continue;
        }

        var item = volumes.Items[0];
        book.Thumbnail = item.VolumeInfo.ImageLinks.Thumbnail;
    }

    exporter.export(books);
}
catch (NotionApiException notionApiException)
{
    Console.WriteLine($"An error occurred communicating with notion: {notionApiException}");
}
catch (IOException ioException)
{
    Console.WriteLine($"An error occurred reading the clippings file: {ioException}");
}

