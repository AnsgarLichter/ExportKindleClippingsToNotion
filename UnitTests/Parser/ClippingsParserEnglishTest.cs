﻿using System.Collections;
using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsParserEnglish))]
public class ClippingsParserEnglishTest
{
    [Fact]
    public void ReturnsNullForAnInvalidClipping()
    {
        var parser = new ClippingsParserEnglish();
        const string clipping = "line1\nline2\nline3";

        Assert.Null(parser.Parse(clipping));
    }

    [Theory]
    [ClassData(typeof(ValidEnglishClippingTestData))]
    public void ReturnsAValidClipping(string clipping, string expectedAuthor, string expectedTitle,
        int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate,
        string expectedText)
    {
        var parser = new ClippingsParserEnglish();
        var result = parser.Parse(clipping);

        Assert.NotNull(result);
        Assert.Equal(expectedAuthor, result.Author);
        Assert.Equal(expectedTitle, result.Title);
        Assert.Equal(expectedStartPosition, result.StartPosition);
        Assert.Equal(expectedFinishPosition, result.FinishPosition);
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedHighlightDate, result.HighlightDate);
        Assert.Equal(expectedText, result.Text);
    }

    [Theory]
    [ClassData(typeof(InvalidEnglishClippingTestData))]
    public void ReturnsBestMatchForInValidClipping(string clipping, string expectedAuthor, string expectedTitle,
        int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate,
        string expectedText)
    {
        var parser = new ClippingsParserEnglish();
        var result = parser.Parse(clipping);

        Assert.NotNull(result);
        Assert.Equal(expectedAuthor, result.Author);
        Assert.Equal(expectedTitle, result.Title);
        Assert.Equal(expectedStartPosition, result.StartPosition);
        Assert.Equal(expectedFinishPosition, result.FinishPosition);
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedHighlightDate, result.HighlightDate);
        Assert.Equal(expectedText, result.Text);
    }
    
    [Fact]
    public void ReturnsNullIfLimitHasBeenReached()
    {
        var clipping = "How To Win Friends and Influence People (Carnegie, Dale)\n- Your Highlight on page 79 | location 1293-1295 | Added on Tuesday, 30 August 2022 19:31:58\n\n <You have reached the clipping limit for this item> ";
        var parser = new ClippingsParserEnglish();

        Assert.Null(parser.Parse(clipping));
    }
    
}

public class ValidEnglishClippingTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "How To Win Friends and Influence People (Carnegie, Dale)\n- Your Highlight on page 79 | location 1293-1295 | Added on Tuesday, 30 August 2022 19:31:58\n\n7\u200b. \u200b\u200b\u200bAn Easy Way to Become a Good Conversationalist\u200b\u200b\n",
            "Carnegie, Dale",
            "How To Win Friends and Influence People",
            1293,
            1295,
            79,
            DateTime.Parse("2022-08-30T19:31:58"),
            "7\u200b. \u200b\u200b\u200bAn Easy Way to Become a Good Conversationalist\u200b\u200b"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class InvalidEnglishClippingTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "Carnegie, Dale (How To Win Friends and Influence People)\n- Your Highlight on page 79 | location 1293-1295 | Added on Tuesday, 30 August 2022 19:31:58\n\n7\u200b. \u200b\u200b\u200bAn Easy Way to Become a Good Conversationalist\u200b\u200b\n",
            "How To Win Friends and Influence People",
            "Carnegie, Dale",
            1293,
            1295,
            79,
            DateTime.Parse("2022-08-30T19:31:58"),
            "7\u200b. \u200b\u200b\u200bAn Easy Way to Become a Good Conversationalist\u200b\u200b"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}