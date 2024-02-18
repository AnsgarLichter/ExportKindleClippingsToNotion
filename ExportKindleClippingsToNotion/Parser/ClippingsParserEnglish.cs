using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsParserEnglish() : ClippingsParser(new ClippingsLanguageConfiguration
{
    Author = new Regex(@"(?<=\()(?!.+?\()(.+?)(?=\))"),
    Title = new Regex(@".+?(?=\s*\()"),
    Date = new Regex(@"\d{2}[a-zA-Z_ .]*\d{4}\s*\d{2}:\d{2}:\d{2}"),
    Page = new Regex("(?<=page )[0-9]*"),
    StartPosition = new Regex("[0-9]+(?=-)"),
    FinishPosition = new Regex("(?<=-)[0-9]+"),
    ClippingsLimitReached = new Regex("<.+?>"),
    CultureInfo = new CultureInfo("en-EN")
});