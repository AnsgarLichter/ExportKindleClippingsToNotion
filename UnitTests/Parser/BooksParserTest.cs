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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.ReturnsNextFromSequence(
            new ClippingDto(text: "text1", startPosition: 1, finishPosition: 2, page: 1,
                highlightDate: new DateTime(2024, 02, 19, 21, 20, 00), author: "author", title: "title"),
            new ClippingDto(text: "text2", startPosition: 1, finishPosition: 2, page: 1,
                highlightDate: new DateTime(2024, 02, 19, 21, 20, 00), author: "author", title: "title"));
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)null);

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
        Assert.Equal(
            "https://bookstoreromanceday.org/wp-content/uploads/2020/08/book-cover-placeholder.png",
            booksEnumerator.Current.ThumbnailUrl
        );
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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.ReturnsNextFromSequence(
            new ClippingDto(text: "text1", startPosition: 1, finishPosition: 2, page: 1,
                highlightDate: new DateTime(2024, 02, 19, 21, 20, 00), author: "author", title: "title"),
            new ClippingDto(text: "text2", startPosition: 1, finishPosition: 2, page: 1,
                highlightDate: new DateTime(2024, 02, 19, 21, 20, 00), author: "author", title: "title"));
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)"https://example.com/thumbnail.jpg");

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
        Assert.Equal("https://example.com/thumbnail.jpg", booksEnumerator.Current.ThumbnailUrl);
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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.ReturnsNextFromSequence(
            new ClippingDto(text: "text1", startPosition: 1, finishPosition: 2, page: 1,
                highlightDate: new DateTime(2024, 02, 19, 21, 20, 00), author: "author1", title: "title1"),
            new ClippingDto(text: "text2", startPosition: 1, finishPosition: 2, page: 1,
                highlightDate: new DateTime(2024, 02, 19, 21, 20, 00), author: "author2", title: "title2"));
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)null);

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
        Assert.Equal(
            "https://bookstoreromanceday.org/wp-content/uploads/2020/08/book-cover-placeholder.png",
            booksEnumerator.Current.ThumbnailUrl
        );
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
        Assert.Equal(
            "https://bookstoreromanceday.org/wp-content/uploads/2020/08/book-cover-placeholder.png",
            booksEnumerator.Current.ThumbnailUrl
        );
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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.Returns((ClippingDto?)null);
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)null);

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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.Returns(
            new ClippingDto(text: "", startPosition: 1, finishPosition: 1, page: 1,
                highlightDate: new DateTime(2024, 02, 27), author: "author", title: "title"));
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)null);

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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.Returns(
            new ClippingDto(text: "", startPosition: 1, finishPosition: 1, page: 1,
                highlightDate: new DateTime(2024, 02, 27), author: null, title: "title"));
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)null);

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

        A.CallTo(() => clippingsParserMock.Parse(A<string>.Ignored))!.Returns(
            new ClippingDto(text: "", startPosition: 1, finishPosition: 1, page: 1,
                highlightDate: new DateTime(2024, 02, 27), author: "author", title: null));
        A.CallTo(() => metadataFetcherMock.GetThumbnailUrlAsync(A<BookDto>.Ignored)).Returns((string?)null);

        var result = await booksParser.ParseAsync(new[]
        {
            "invalidClipping1",
            "invalidClipping2"
        });

        Assert.Empty(result);
    }
}