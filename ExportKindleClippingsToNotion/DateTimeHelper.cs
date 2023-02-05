using System.Globalization;

namespace ExportKindleClippingsToNotion;

public static class DateTimeHelper
{
    public static DateTime ParseAnyCulture(string dateAsString)
    {
        return CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Select(culture =>
            {
                DateTime.TryParse(
                    dateAsString,
                    culture,
                    DateTimeStyles.None,
                    out var result
                );

                return result;
            })
            .Where(date => date != default)
            .GroupBy(date => date)
            .MaxBy(group => group.Count())
            ?.Key ?? default;
    }
}