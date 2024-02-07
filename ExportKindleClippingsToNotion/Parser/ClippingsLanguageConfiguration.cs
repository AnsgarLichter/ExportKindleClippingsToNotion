using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion.Parser;

public record ClippingsLanguageConfiguration
{
    public required Regex Author { get; init; }
    public required Regex Title { get; init; }
    public required Regex Date { get; init; }
    public required Regex Page { get; init; }
    public required Regex StartPosition { get; init; }
    public required Regex FinishPosition { get; init; }
    public required Regex ClippingsLimitReached { get; init; }

    public required CultureInfo CultureInfo { get; init; }
}