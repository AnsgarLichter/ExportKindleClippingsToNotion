using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsParserRussian : ClippingsParser
{
    public ClippingsParserRussian() : base(new ClippingsLanguageConfiguration
    {
        Author = new Regex(@"(?<=\()(?!.+?\()(.+?)(?=\))"),
        Title = new Regex(@"^[^(]+?(?=\s\()"),
        Date = new Regex(@"(?<=Добавлено: )(\p{L}+, \d{1,2} \p{L}+ \d{4}) г. в \d{1,2}:\d{2}:\d{2}"),
        Page = new Regex("(?<=странице )[0-9]*"),
        StartPosition =  new Regex(@"(?<=месте\s|Место\s)\d+"),
        FinishPosition = new Regex("(?<=–)[0-9]+"),
        ClippingsLimitReached = new Regex("<.+?>"),
        CultureInfo = new CultureInfo("ru-RU")
    })
    {
        var dateTimeFormat = LanguageConfiguration.CultureInfo.DateTimeFormat;
        dateTimeFormat.LongDatePattern = "dddd, d MMMM yyyy 'г. в'";
        dateTimeFormat.LongTimePattern = "HH:mm:ss";
    }
}