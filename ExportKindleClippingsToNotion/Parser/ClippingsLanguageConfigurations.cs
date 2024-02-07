﻿using System.Globalization;
using System.Text.RegularExpressions;

namespace ExportKindleClippingsToNotion.Parser;

public static class ClippingsLanguageConfigurations
{
    public static readonly ClippingsLanguageConfiguration English = new ClippingsLanguageConfiguration
    {
        Author = new Regex(@"(?<=\()(?!.+?\()(.+?)(?=\))"),
        Title = new Regex(@".+?(?=\s*\()"),
        Date = new Regex(@"\d{2}[a-zA-Z_ .]*\d{4}\s*\d{2}:\d{2}:\d{2}"),
        Page = new Regex("(?<=Seite )[0-9]*"),
        StartPosition = new Regex("[0-9]+(?=-)"),
        FinishPosition = new Regex("(?<=-)[0-9]+"),
        ClippingsLimitReached = new Regex("<.+?>"),
        CultureInfo = new CultureInfo("en-EN")
    };

    public static readonly ClippingsLanguageConfiguration German = new ClippingsLanguageConfiguration
    {
        Author = new Regex(@"(?<=\()(?!.+?\()(.+?)(?=\))"),
        Title = new Regex(@".+?(?=\s*\()"),
        Date = new Regex(@"\d{2}[a-zA-Z_ .]*\d{4}\s*\d{2}:\d{2}:\d{2}"),
        Page = new Regex("(?<=Seite )[0-9]*"),
        StartPosition = new Regex("[0-9]+(?=-)"),
        FinishPosition = new Regex("(?<=-)[0-9]+"),
        ClippingsLimitReached = new Regex("<.+?>"),
        CultureInfo = new CultureInfo("de-DE")
    };
}