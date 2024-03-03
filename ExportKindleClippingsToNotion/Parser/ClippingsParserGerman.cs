using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsParserGerman() : ClippingsParser(new ClippingsLanguageConfiguration
{
    Author = new Regex(@"(?<=\()(?!.+?\()(.+?)(?=\))"),
    Title = new Regex(@"^[^(]+?(?=\s\()"),
    Date = new Regex(@"(\d+)\. (\w+) (\d+) (\d+:\d+:\d+)"),
    Page = new Regex("(?<=Seite )[0-9]*"),
    StartPosition = new Regex("[0-9]+(?=-)"),
    FinishPosition = new Regex("(?<=-)[0-9]+"),
    ClippingsLimitReached = new Regex("<.+?>"),
    CultureInfo = new CultureInfo("de-DE")
});