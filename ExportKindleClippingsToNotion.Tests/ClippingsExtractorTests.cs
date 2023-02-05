using FluentAssertions;

namespace ExportKindleClippingsToNotion.Tests;

public class ClippingsExtractorTests
{
    [Theory]
    [InlineData("./Assets/Test1_DE.txt")]
    [InlineData("./Assets/Test1_EN.txt")]
    [InlineData("./Assets/Test1_ES.txt")]
    [InlineData("./Assets/Test1_NL.txt")]
    public void Should_Work_With_Different_Languages(string filePath)
    {
        var sut = new ClippingsExtractor();

        var fileContent = File.ReadAllText(filePath);

        var clippings = sut.ExtractClippings(fileContent);

        clippings.Should().HaveCount(1);
        
        var clipping = clippings.First();
        
        clipping.Book!.Title.Should().Be("How To Win Friends and Influence People");
        clipping.Book.Author.Should().Be("Carnegie, Dale");
        clipping.Page.Should().Be(79);
        clipping.StartPosition.Should().Be(1293);
        clipping.FinishPosition.Should().Be(1295);
        clipping.HighlightDate.Should().Be(new DateTime(2022,8,30,19,46,26));
        clipping.Text.Should().Be("7. An Easy Way to Become a Good Conversationalist");
    }
    
    [Fact]
    public void Should_Work_With_Variations()
    {
        var sut = new ClippingsExtractor();

        var fileContent = File.ReadAllText("./Assets/Test2_DE.txt");

        var clippings = sut.ExtractClippings(fileContent);

        clippings.Should().HaveCount(4);
        
        var clipping1 = clippings[0];
        
        clipping1.Book!.Title.Should().Be("Patterns of enterprise application architecture-Addison-Wesley");
        clipping1.Book.Author.Should().Be("Fowler, Martin");
        clipping1.Page.Should().Be(0);
        clipping1.StartPosition.Should().Be(986);
        clipping1.FinishPosition.Should().Be(987);
        clipping1.HighlightDate.Should().Be(new DateTime(2021,7,14,19,6,11));
        clipping1.Text.Should().Be("corresponds pretty closely to the database structure, with one domain class per database table. Such domain objects often have only moderately complex business logic.");
        
        var clipping2 = clippings[1];
        
        clipping2.Book!.Title.Should().Be("Souverän investieren mit Indexfonds und ETFs (German Edition)");
        clipping2.Book.Author.Should().Be("Kommer, Gerd");
        clipping2.Page.Should().Be(178);
        clipping2.StartPosition.Should().Be(4136);
        clipping2.FinishPosition.Should().Be(4137);
        clipping2.HighlightDate.Should().Be(new DateTime(2022,1,26,22,18,38));
        clipping2.Text.Should().Be("<Sie haben die maximale Anzahl an Clipboard-Einträgen für diesen Inhalt erreicht>");

        var clipping3 = clippings[2];
        
        clipping3.Book!.Title.Should().Be("Why We Sleep: Unlocking the Power of Sleep and Dreams");
        clipping3.Book.Author.Should().Be("Matthew Walker");
        clipping3.Page.Should().Be(0);
        clipping3.StartPosition.Should().Be(1420);
        clipping3.FinishPosition.Should().Be(1421);
        clipping3.HighlightDate.Should().Be(new DateTime(2021,8,25,16,11,46));
        clipping3.Text.Should().Be("late childhood and adolescence.");

        var clipping4 = clippings[3];
        
        clipping4.Book!.Title.Should().Be("Clean Architecture");
        clipping4.Book.Author.Should().Be("Robert C. Martin");
        clipping4.Page.Should().Be(0);
        clipping4.StartPosition.Should().Be(2983);
        clipping4.FinishPosition.Should().Be(2984);
        clipping4.HighlightDate.Should().Be(new DateTime(2022,5,17,21,0,35));
        clipping4.Text.Should().Be("initial entry point of the system.");

    }
    
    [Fact(Skip = "WIP")]
    public void Should_Work_With_Variations2()
    {
        var sut = new ClippingsExtractor();

        var fileContent = File.ReadAllText("./Assets/Test3_DE.txt");

        var clippings = sut.ExtractClippings(fileContent);

        clippings.Should().HaveCount(4);
        
        var clipping1 = clippings[0];
        
        clipping1.Book!.Title.Should().Be("Clean Code A Handbook of Agile Software Craftsmanship-Prentice Hall");
        clipping1.Book.Author.Should().Be("Robert C. Martin");
        clipping1.Page.Should().Be(0);
        clipping1.StartPosition.Should().Be(1138);
        clipping1.FinishPosition.Should().Be(1138);
        clipping1.HighlightDate.Should().Be(new DateTime(2021,7,3,17,52,9));
        clipping1.Text.Should().Be("you first look at the method, the meanings of the variables are opaque.");

    }
}