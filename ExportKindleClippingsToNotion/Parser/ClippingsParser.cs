using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Model.Dto;

namespace ExportKindleClippingsToNotion.Parser;

public abstract class ClippingsParser(ClippingsLanguageConfiguration languageConfiguration) : IClippingsParser
{
    public Task<ClippingDto?> ParseAsync(string clipping)
    {
        var lines = clipping.Split("\n");
        if (lines.Length < 4)
        {
            Console.WriteLine("Found an invalid clipping. Parsing next one ...");
            return Task.FromResult<ClippingDto?>(null);
        }
        
        var lineTitleAndAuthor = lines[0];
        var title = languageConfiguration.Title.Match(lineTitleAndAuthor).Value;
        var author = languageConfiguration.Author.Match(lineTitleAndAuthor).Value;
        
        var linePagePositionDate = lines[1];
        var page = languageConfiguration.Page.Match(linePagePositionDate).Value;
        var startPosition = languageConfiguration.StartPosition.Match(linePagePositionDate).Value;
        var finishPosition = languageConfiguration.FinishPosition.Match(linePagePositionDate).Value;
        var date = languageConfiguration.Date.Match(linePagePositionDate).Value;
        // TODO: date string may be null - should be validated before
        var dateTime = DateTime.Parse(date, languageConfiguration.CultureInfo);
        var text = lines[3];
        if (languageConfiguration.ClippingsLimitReached.IsMatch(text))
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
        return Task.FromResult<ClippingDto?>(new ClippingDto(parsedClipping, author, title));
    }
}