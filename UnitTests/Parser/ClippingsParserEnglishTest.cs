using System.Collections;
using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsParserEnglish))]
public class ClippingsParserEnglishTest
{
    [Fact]
    public async Task ReturnsNullForAnInvalidClipping()
    {
        var parser = new ClippingsParserEnglish();
        const string clipping = "line1\nline2\nline3";

        Assert.Null(await parser.ParseAsync(clipping));
    }

    [Theory]
    [ClassData(typeof(ValidEnglishClippingTestData))]
    public async Task ReturnsAValidClipping(string clipping, string expectedAuthor, string expectedTitle,
        int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate,
        string expectedText)
    {
        var parser = new ClippingsParserEnglish();
        var result = await parser.ParseAsync(clipping);

        Assert.NotNull(result);
        Assert.Equal(expectedAuthor, result.Author);
        Assert.Equal(expectedTitle, result.Title);
        Assert.NotNull(result.Clipping);
        Assert.Equal(expectedStartPosition, result.Clipping.StartPosition);
        Assert.Equal(expectedFinishPosition, result.Clipping.FinishPosition);
        Assert.Equal(expectedPage, result.Clipping.Page);
        Assert.Equal(expectedHighlightDate, result.Clipping.HighlightDate);
        Assert.Equal(expectedText, result.Clipping.Text);
    }

    [Theory]
    [ClassData(typeof(InvalidEnglishClippingTestData))]
    public async Task ReturnsBestMatchForInValidClipping(string clipping, string expectedAuthor, string expectedTitle,
        int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate,
        string expectedText)
    {
        var parser = new ClippingsParserEnglish();
        var result = await parser.ParseAsync(clipping);

        Assert.NotNull(result);
        Assert.Equal(expectedAuthor, result.Author);
        Assert.Equal(expectedTitle, result.Title);
        Assert.NotNull(result.Clipping);
        Assert.Equal(expectedStartPosition, result.Clipping.StartPosition);
        Assert.Equal(expectedFinishPosition, result.Clipping.FinishPosition);
        Assert.Equal(expectedPage, result.Clipping.Page);
        Assert.Equal(expectedHighlightDate, result.Clipping.HighlightDate);
        Assert.Equal(expectedText, result.Clipping.Text);
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