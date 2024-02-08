using System.Collections;
using System.Diagnostics;
using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsParserGerman))]
public class ClippingsParserGermanTest
{

    [Fact]
    public async Task ReturnsNullForAnInvalidClipping()
    {
        var parser = new ClippingsParserGerman();
        const string clipping = "line1\nline2\nline3";
        
        Assert.Null(await parser.ParseAsync(clipping));
    }
    
    [Theory]
    [ClassData(typeof(ValidClippingTestData))]
    public async Task ReturnsAValidClipping(string clipping, string expectedAuthor, string expectedTitle, int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate, string expectedText)
    {
        var parser = new ClippingsParserGerman();
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

public class ValidClippingTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "Clean Architecture (Robert C. Martin)\n- Ihre Markierung bei Position 2983-2984 | Hinzugefügt am Dienstag, 17. Mai 2022 21:00:35\n\ninitial entry point of the system.", 
            "Robert C. Martin", 
            "Clean Architecture", 
            2983, 
            2984, 
            0, 
            DateTime.Parse("2022-05-17T21:00:35"), 
            "initial entry point of the system."
        };
        yield return new object[]
        {
            "Why We Sleep: Unlocking the Power of Sleep and Dreams (Matthew Walker)\n- Ihre Markierung bei Position 1420-1421 | Hinzugefügt am Mittwoch, 25. August 2021 16:11:46\n\nlate childhood and adolescence.", 
            "Matthew Walker", 
            "Why We Sleep: Unlocking the Power of Sleep and Dreams", 
            1420, 
            1421, 
            0, 
            DateTime.Parse("2021-08-25T16:11:46"), 
            "late childhood and adolescence."
        };
        yield return new object[]
        {
            "Patterns of enterprise application architecture (2015) (Fowler, Martin)\n- Ihre Markierung bei Position 986-987 | Hinzugefügt am Mittwoch, 14. Juli 2021 19:06:11\n\ncorresponds pretty closely to the database structure, with one domain class per database table. Such domain objects often have only moderately complex business logic.", 
            "Fowler, Martin", 
            "Patterns of enterprise application architecture", 
            986, 
            987, 
            0, 
            DateTime.Parse("2021-07-14T19:06:11"), 
            "corresponds pretty closely to the database structure, with one domain class per database table. Such domain objects often have only moderately complex business logic."
        };
        yield return new object[]
        {
            "Robert C. Martin (Clean Code A Handbook of Agile Software Craftsmanship-Prentice Hall (2008))\n- Ihre Markierung bei Position 1138-1138 | Hinzugefügt am Samstag, 3. Juli 2021 17:52:09\n\nyou first look at the method, the meanings of the variables are opaque.", 
            "Clean Code A Handbook of Agile Software Craftsmanship-Prentice Hall (2008)", 
            "Robert C. Martin", 
            1138, 
            1138, 
            0, 
            DateTime.Parse("2021-07-03T17:52:09"), 
            "you first look at the method, the meanings of the variables are opaque."
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}