using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Parser;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(BooksParserFactory))]
public class BooksParserFactoryTest
{

    [Fact]
    public void BooksParserIsReturned()
    {   
        var metadataFetcher = A.Fake<IBookMetadataFetcher>();
        var testSubject = new BooksParserFactory(metadataFetcher);
        
        var clippingsParser = A.Fake<IClippingsParser>();
        var booksParser = testSubject.Create(clippingsParser);
        
        Assert.NotNull(booksParser);
    }
}