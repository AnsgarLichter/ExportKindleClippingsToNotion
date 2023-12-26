﻿using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsLanguage))]
public class ClippingsLanguageTest
{
    private const string EnglishClipping = """
                                           How To Win Friends and Influence People (Carnegie, Dale)
                                           - Your Highlight on page 79 | location 1293-1295 | Added on Tuesday, 30 August 2022 19:31:58

                                           7. ​​​An Easy Way to Become a Good Conversationalist​​
                                           ==========
                                           """;

    private const string GermanClipping = """
                                          Robert C. Martin (Clean Code A Handbook of Agile Software Craftsmanship-Prentice Hall (2008))
                                          - Ihre Markierung bei Position 1138-1138 | Hinzugefügt am Samstag, 3. Juli 2021 17:52:09

                                          you first look at the method, the meanings of the variables are opaque.
                                          ==========
                                          """;

    private const string SpanishClipping = """
                                           How To Win Friends and Influence People (Carnegie, Dale)
                                           - La subrayado en la página 79 | posición 1293-1295 | Añadido el martes, 30 de agosto de 2022 19:40:15

                                           7​. ​​​An Easy Way to Become a Good Conversationalist​​
                                           ==========
                                           """;

    private const string UnknownClipping = """
                                           How To Win Friends and Influence People (Carnegie, Dale)
                                           - asdasdasdlösajdsad 79 | asdasdasdsa 1293-1295 | sadaasdasds, 30 asdasdas 2022 19:40:15

                                           7​. ​​​An Easy Way to Become a Good Conversationalist​​
                                           ==========
                                           """;


    [Fact]
    public void ReturnsEnglish()
    {
        var clippingsLanguage = new ClippingsLanguage();
        var determinedLanguage = clippingsLanguage.Determine(EnglishClipping);

        Assert.Equal(SupportedLanguages.English, determinedLanguage);
    }

    [Fact]
    public void ReturnsGerman()
    {
        var clippingsLanguage = new ClippingsLanguage();
        var determinedLanguage = clippingsLanguage.Determine(GermanClipping);

        Assert.Equal(SupportedLanguages.German, determinedLanguage);
    }

    [Fact]
    public void ReturnsNotSupportedForSpanish()
    {
        var clippingsLanguage = new ClippingsLanguage();

        var exception =
            Assert.Throws<LanguageNotSupportedException>(() => clippingsLanguage.Determine(SpanishClipping));

        Assert.NotNull(exception);
        Assert.Equal($"Spanish is currently not supported!", exception.Message);
    }

    [Fact]
    public void ReturnsNotRecognizedForUnknownLanguage()
    {
        var clippingsLanguage = new ClippingsLanguage();

        var exception =
            Assert.Throws<LanguageNotRecognizedException>(() => clippingsLanguage.Determine(UnknownClipping));

        Assert.NotNull(exception);
        Assert.Equal($"The language of the clipping couldn't be determined!", exception.Message);
    }
}