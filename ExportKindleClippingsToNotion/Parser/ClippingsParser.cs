using ExportKindleClippingsToNotion.Model.Dto;

namespace ExportKindleClippingsToNotion.Parser;

public abstract class ClippingsParser : IClippingsParser
{
    protected readonly ClippingsLanguageConfiguration LanguageConfiguration;

    protected ClippingsParser(ClippingsLanguageConfiguration languageConfiguration)
    {
        LanguageConfiguration = languageConfiguration;
    }

    public ClippingDto? Parse(string clipping)
    {
        var lines = clipping.Split("\n");
        if (lines.Length < 4)
        {
            Console.WriteLine("Found an invalid clipping. Parsing next one ...");
            return null;
        }

        var lineTitleAndAuthor = lines[0];
        var title = LanguageConfiguration.Title.Match(lineTitleAndAuthor).Value;
        var author = LanguageConfiguration.Author.Match(lineTitleAndAuthor).Value;

        var linePagePositionDate = lines[1];
        var page = LanguageConfiguration.Page.Match(linePagePositionDate).Value;
        var startPosition = LanguageConfiguration.StartPosition.Match(linePagePositionDate).Value;
        var finishPosition = LanguageConfiguration.FinishPosition.Match(linePagePositionDate).Value;
        var date = LanguageConfiguration.Date.Match(linePagePositionDate).Value;
        var dateTime = date.Trim().Equals("") ? DateTime.Now : DateTime.Parse(date, LanguageConfiguration.CultureInfo);
        var text = lines[3];
        if (LanguageConfiguration.ClippingsLimitReached.IsMatch(text))
        {
            Console.WriteLine("Skipping clipping because limit has been reached");
            return null;
        }

        Console.WriteLine(
            $"{title} by {author}: Page {page} at position from {startPosition} to {finishPosition} created at {dateTime} - {text}");

        return new ClippingDto(text: text,
            startPosition: !string.IsNullOrEmpty(startPosition) ? int.Parse(startPosition) : 0,
            finishPosition: !string.IsNullOrEmpty(finishPosition) ? int.Parse(finishPosition) : 0,
            page: !string.IsNullOrEmpty(page) ? int.Parse(page) : 0, highlightDate: dateTime, author: author,
            title: title);
    }
}