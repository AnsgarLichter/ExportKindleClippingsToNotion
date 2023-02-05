using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion;

public class ClippingsExtractor
{
    public IReadOnlyList<Clipping> ExtractClippings(string file)
    {
        var clippingSections = file
            .Split("==========")
            .Where(clipping => clipping.Trim().Length > 0);

        var clippings = new List<Clipping>();

        foreach (var clippingSection in clippingSections)
        {
            var lines = clippingSection
                .Split("\r\n")
                .Where(line => line.Length > 0)
                .Select(line => line.Trim())
                .Select(line => line.Replace("\u200B", string.Empty))
                .Select(line => line.Replace("\uFEFF", string.Empty))
                .ToList();

            if (lines.Count != 3)
                continue;

            var firstLineRegex = new Regex(@"^(?<Title>.*?)(\((?<Year>\d{4})\) )?\((?<AuthorName>[^\(]*?)\)$");
            var firstLineRegexMatch = firstLineRegex.Match(lines[0]);
            
            var title = firstLineRegexMatch.Groups["Title"].Value.Trim();
            var authorName = firstLineRegexMatch.Groups["AuthorName"].Value.Trim();
            var yearAsString = firstLineRegexMatch.Groups["Year"].Value.Trim();
            int.TryParse(yearAsString, out var year);

            
            var secondLineRegex = new Regex(@"^- (.*?(?<Page>\d+) \| )?.*?(?<PositionStart>\d+).*?(?<PositionEnd>\d+) \| .*?(?<Date>\d+.*?)$");
            var secondLineRegexMatch = secondLineRegex.Match(lines[1]);
            
            var pageAsString = secondLineRegexMatch.Groups["Page"].Value;
            int.TryParse(pageAsString, out var page);
            var positionStartAsString = secondLineRegexMatch.Groups["PositionStart"].Value;
            int.TryParse(positionStartAsString, out var positionStart);
            var positionEndAsString = secondLineRegexMatch.Groups["PositionEnd"].Value;
            int.TryParse(positionEndAsString, out var positionEnd);
            var dateAsString = secondLineRegexMatch.Groups["Date"].Value;
            var date = DateTimeHelper.ParseAnyCulture(dateAsString);

            
            var text = lines[2];

            
            var clipping = new Clipping(text, positionStart, positionEnd, page, date)
            {
                Book = new Book(authorName, title)
            };

            clippings.Add(clipping);
        }

        return clippings;
    }
}