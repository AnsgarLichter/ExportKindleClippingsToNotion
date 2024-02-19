using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Model.Dto;
using ExportKindleClippingsToNotion.Parser;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(BooksParser))]
public class BooksParserTest
{
    [Fact]
    public async Task ReturnASingleBookWithFallbackThumbnailContainingAllClippings()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.ReturnsNextFromSequence(
            new ClippingDto(new Clipping(
                    "text1",
                    1,
                    2,
                    1,
                    new DateTime(2024, 02, 19, 21, 20, 00)),
                "author",
                "title"
            ),
            new ClippingDto(new Clipping(
                    "text2",
                    1,
                    2,
                    1,
                    new DateTime(2024, 02, 19, 21, 20, 00)),
                "author",
                "title"
            )
        );
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "validClipping1",
            "validClipping2"
        });

        Assert.NotNull(result);
        Assert.Single(result);
        var booksEnumerator = result.GetEnumerator();
        booksEnumerator.MoveNext();
        Assert.Equal("title", booksEnumerator.Current.Title);
        Assert.Equal("author", booksEnumerator.Current.Author);
        Assert.Null(booksEnumerator.Current.Thumbnail);
        Assert.Equal(2, booksEnumerator.Current.Clippings.Count);
        var clippingsEnumerator = booksEnumerator.Current.Clippings.GetEnumerator();
        clippingsEnumerator.MoveNext();
        Assert.Equal(booksEnumerator.Current, clippingsEnumerator.Current.Book);
        Assert.Equal("text1", clippingsEnumerator.Current.Text);
        Assert.Equal(1, clippingsEnumerator.Current.StartPosition);
        Assert.Equal(2, clippingsEnumerator.Current.FinishPosition);
        Assert.Equal(1, clippingsEnumerator.Current.Page);
        clippingsEnumerator.MoveNext();
        Assert.Equal(booksEnumerator.Current, clippingsEnumerator.Current.Book);
        Assert.Equal("text2", clippingsEnumerator.Current.Text);
        Assert.Equal(1, clippingsEnumerator.Current.StartPosition);
        Assert.Equal(2, clippingsEnumerator.Current.FinishPosition);
        Assert.Equal(1, clippingsEnumerator.Current.Page);
    }

    [Fact]
    public async Task ReturnASingleBookWithFetchedThumbnailContainingAllClippings()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.ReturnsNextFromSequence(
            new ClippingDto(new Clipping(
                    "text1",
                    1,
                    2,
                    1,
                    new DateTime(2024, 02, 19, 21, 20, 00)),
                "author",
                "title"
            ),
            new ClippingDto(new Clipping(
                    "text2",
                    1,
                    2,
                    1,
                    new DateTime(2024, 02, 19, 21, 20, 00)),
                "author",
                "title"
            )
        );
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)"thumbnail");

        var result = await booksParser.ParseAsync(new[]
        {
            "validClipping1",
            "validClipping2"
        });

        Assert.NotNull(result);
        Assert.Single(result);
        var booksEnumerator = result.GetEnumerator();
        booksEnumerator.MoveNext();
        Assert.Equal("title", booksEnumerator.Current.Title);
        Assert.Equal("author", booksEnumerator.Current.Author);
        Assert.Equal("thumbnail", booksEnumerator.Current.Thumbnail);
        Assert.Equal(2, booksEnumerator.Current.Clippings.Count);
        var clippingsEnumerator = booksEnumerator.Current.Clippings.GetEnumerator();
        clippingsEnumerator.MoveNext();
        Assert.Equal(booksEnumerator.Current, clippingsEnumerator.Current.Book);
        Assert.Equal("text1", clippingsEnumerator.Current.Text);
        Assert.Equal(1, clippingsEnumerator.Current.StartPosition);
        Assert.Equal(2, clippingsEnumerator.Current.FinishPosition);
        Assert.Equal(1, clippingsEnumerator.Current.Page);
        clippingsEnumerator.MoveNext();
        Assert.Equal(booksEnumerator.Current, clippingsEnumerator.Current.Book);
        Assert.Equal("text2", clippingsEnumerator.Current.Text);
        Assert.Equal(1, clippingsEnumerator.Current.StartPosition);
        Assert.Equal(2, clippingsEnumerator.Current.FinishPosition);
        Assert.Equal(1, clippingsEnumerator.Current.Page);
    }

    [Fact]
    public async Task ReturnsTwoBooksWithOneClippingPerBook()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.ReturnsNextFromSequence(
            new ClippingDto(new Clipping(
                    "text1",
                    1,
                    2,
                    1,
                    new DateTime(2024, 02, 19, 21, 20, 00)),
                "author1",
                "title1"
            ),
            new ClippingDto(new Clipping(
                    "text2",
                    1,
                    2,
                    1,
                    new DateTime(2024, 02, 19, 21, 20, 00)),
                "author2",
                "title2"
            )
        );
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "validClipping1",
            "validClipping2"
        });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        var booksEnumerator = result.GetEnumerator();
        booksEnumerator.MoveNext();
        Assert.Equal("title1", booksEnumerator.Current.Title);
        Assert.Equal("author1", booksEnumerator.Current.Author);
        Assert.Null(booksEnumerator.Current.Thumbnail);
        Assert.Equal(1, booksEnumerator.Current.Clippings.Count);
        var clippingsEnumerator = booksEnumerator.Current.Clippings.GetEnumerator();
        clippingsEnumerator.MoveNext();
        Assert.Equal(booksEnumerator.Current, clippingsEnumerator.Current.Book);
        Assert.Equal("text1", clippingsEnumerator.Current.Text);
        Assert.Equal(1, clippingsEnumerator.Current.StartPosition);
        Assert.Equal(2, clippingsEnumerator.Current.FinishPosition);
        Assert.Equal(1, clippingsEnumerator.Current.Page);

        booksEnumerator.MoveNext();
        Assert.Equal("title2", booksEnumerator.Current.Title);
        Assert.Equal("author2", booksEnumerator.Current.Author);
        Assert.Null(booksEnumerator.Current.Thumbnail);
        Assert.Equal(1, booksEnumerator.Current.Clippings.Count);
        clippingsEnumerator = booksEnumerator.Current.Clippings.GetEnumerator();
        clippingsEnumerator.MoveNext();
        Assert.Equal(booksEnumerator.Current, clippingsEnumerator.Current.Book);
        Assert.Equal("text2", clippingsEnumerator.Current.Text);
        Assert.Equal(1, clippingsEnumerator.Current.StartPosition);
        Assert.Equal(2, clippingsEnumerator.Current.FinishPosition);
        Assert.Equal(1, clippingsEnumerator.Current.Page);
    }
    
    [Fact]
    public async Task ReturnsNullForInvalidClippings()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.Returns((ClippingDto?)null);
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "invalidClipping1",
            "invalidClipping2"
        });

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ReturnsNullForClippingsWithoutMarkedText()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.Returns((ClippingDto?)new ClippingDto(null, "author", "title"));
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "invalidClipping1",
            "invalidClipping2"
        });

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ReturnsNullForClippingsWithoutAuthor()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.Returns((ClippingDto?)new ClippingDto(new Clipping("text", 1, 1, 1 , new DateTime()), null, "title"));
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "invalidClipping1",
            "invalidClipping2"
        });

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task ReturnsNullForClippingsWithoutTitle()
    {
        var metadataFetcherMock = A.Fake<IBookMetadataFetcher>();
        var clippingsParserMock = A.Fake<IClippingsParser>();
        var booksParser = new BooksParser(metadataFetcherMock, clippingsParserMock);

        A.CallTo(() => clippingsParserMock.ParseAsync(A<string>.Ignored))!.Returns((ClippingDto?)new ClippingDto(new Clipping("text", 1, 1, 1 , new DateTime()), "author", null));
        A.CallTo(() => metadataFetcherMock.SearchThumbnail(A<Book>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "invalidClipping1",
            "invalidClipping2"
        });

        Assert.Empty(result);
    }
}