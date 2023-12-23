using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion;

public interface IClippingsParser
{
    public Task<ClippingDto?> Parse(string clipping);
}

public class ClippingsParser: IClippingsParser
{
    
    private const string RegexAuthor = "(?<=\\()(?!.+?\\()(.+?)(?=\\))";
    private const string RegexTitle = ".+?(?=\\s*\\()";
    private const string RegexPage = "(?<=Seite )[0-9]*";
    private const string RegexStartPosition = "[0-9]+(?=-)";

    private const string RegexFinishPosition = "(?<=-)[0-9]+";
    
    //TODO: Build Factory which returns regex based on clipping's language
    //TODO: Date Regex not working in every case
    private const string RegexDate = "\\d{2}[a-zA-Z_ .]*\\d{4}\\s*\\d{2}:\\d{2}:\\d{2}";
    private const string RegexClippingsLimitReached = "<.+?>";
    
    public Task<ClippingDto?> Parse(string clipping)
    {
        var lines = clipping.Split("\n");
        if (lines.Length < 4)
        {
            Console.WriteLine("Found an invalid clipping. Parsing next one ...");
            return Task.FromResult<ClippingDto?>(null);
        }
    
        //TODO: Parser for every language - loop over them to find a suitable one and use this one to parse
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
            return Task.FromResult<ClippingDto?>(null);
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
        var dto = new ClippingDto(parsedClipping, author, title);

        return Task.FromResult<ClippingDto?>(dto);
    }
}