using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Model.Dto;

namespace ExportKindleClippingsToNotion.Parser;

public abstract class ClippingsParser(ClippingsLanguageConfiguration languageConfiguration) : IClippingsParser
{
    public ClippingDto? Parse(string clipping)
    {
        var lines = clipping.Split("\n");
        if (lines.Length < 4)
        {
            Console.WriteLine("Found an invalid clipping. Parsing next one ...");
            return null;
        }
        
        var lineTitleAndAuthor = lines[0];
        var title = languageConfiguration.Title.Match(lineTitleAndAuthor).Value;
        var author = languageConfiguration.Author.Match(lineTitleAndAuthor).Value;
        
        var linePagePositionDate = lines[1];
        var page = languageConfiguration.Page.Match(linePagePositionDate).Value;
        var startPosition = languageConfiguration.StartPosition.Match(linePagePositionDate).Value;
        var finishPosition = languageConfiguration.FinishPosition.Match(linePagePositionDate).Value;
        var date = languageConfiguration.Date.Match(linePagePositionDate).Value;
        var dateTime = date.Trim().Equals("") ? DateTime.Now : DateTime.Parse(date, languageConfiguration.CultureInfo);
        var text = lines[3];
        if (languageConfiguration.ClippingsLimitReached.IsMatch(text))
        {
            Console.WriteLine("Skipping clipping because limit has been reached");
            return null;
        }

        Console.WriteLine(
            $"{title} by {author}: Page {page} at position from {startPosition} to {finishPosition} created at {dateTime} - {text}");
        
        return new ClippingDto(
            text,
            !string.IsNullOrEmpty(startPosition) ? int.Parse(startPosition) : 0,
            !string.IsNullOrEmpty(finishPosition) ? int.Parse(finishPosition) : 0,
            !string.IsNullOrEmpty(page) ? int.Parse(page) : 0,
            dateTime,
            author, 
            title
        );
    }
}