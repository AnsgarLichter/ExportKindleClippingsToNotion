using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion;

public class FileClient : IImportClient
{
    
    private const string RegexAuthor = "(?<=\\()(?!.+?\\()(.+?)(?=\\))";
    private const string RegexTitle = ".+?(?=\\s*\\()";
    private const string RegexPage = "(?<=Seite )[0-9]*";
    private const string RegexStartPosition = "[0-9]+(?=-)";

    private const string RegexFinishPosition = "(?<=-)[0-9]+";

    //TODO: Date Regex not working in every case
    private const string RegexDate = "\\d{2}[a-zA-Z_ .]*\\d{4}\\s*\\d{2}:\\d{2}:\\d{2}";
    private const string RegexClippingsLimitReached = "<.+?>";

    public Task<List<Book>> Import(string pathToClippings)
        {
            var clippings = GetParsedClippings(pathToClippings);
            Console.WriteLine($"Determined {clippings.Length} clippings");

            return Task.FromResult(ParseBooks(clippings));
        }

        private static string[] GetParsedClippings(string pathToClippings)
        {
            var clippingsText = File.ReadAllText(pathToClippings);
            return ParseClippings(clippingsText);
        }

        private static string[] ParseClippings(string clippingsText)
        {
            var clippings = clippingsText.Split($"==========\r\n");
            Console.WriteLine($"Determined {clippings.Length} clippings");
            return clippings;
        }

        private static List<Book> ParseBooks(string[] clippings)
        {
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
                var title = Regex.Match(lineTitleAndAuthor, RegexTitle).Value;
                var author = Regex.Match(lineTitleAndAuthor, RegexAuthor).Value;

                var linePagePositionDate = lines[1];
                var page = Regex.Match(linePagePositionDate, RegexPage).Value;
                var startPosition = Regex.Match(linePagePositionDate, RegexStartPosition).Value;
                var finishPosition = Regex.Match(linePagePositionDate, RegexFinishPosition).Value;
                var date = Regex.Match(linePagePositionDate, RegexDate).Value;

                //TODO: Try to determine language automatically
                var dateTime = DateTime.Parse(date, new CultureInfo("de-DE"));

                var text = lines[3];
                if (Regex.IsMatch(text, RegexClippingsLimitReached))
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
            
            return books;
        }
    }