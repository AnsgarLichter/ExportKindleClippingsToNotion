using ExportKindleClippingsToNotion.Model;
using JetBrains.Annotations;

namespace UnitTests.Model;

[TestSubject(typeof(Book))]
public class BookTest
{
    [Fact]
    public void Constructor_SetsAuthorAndTitle()
    {
        const string author = "Author";
        const string title = "Title";

        var book = new Book(author, title);

        Assert.Equal(author, book.Author);
        Assert.Equal(title, book.Title);
    }

    [Fact]
    public void Thumbnail_GetterAndSetter_WorkAsExpected()
    {
        var book = new Book("Author", "Title");
        const string expectedThumbnail = "thumbnail";

        book.Thumbnail = expectedThumbnail;
        var retrievedThumbnail = book.Thumbnail;

        Assert.Equal(expectedThumbnail, retrievedThumbnail);
    }

    [Fact]
    public void AddClipping_AddsClippingToList()
    {
        var book = new Book("Author", "Title");
        var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1), book);

        book.AddClipping(clipping);

        Assert.Contains(clipping, book.Clippings);
    }

    [Fact]
    public void AddClipping_DoesNotAddDuplicateClipping()
    {
        var book = new Book("Author", "Title");
        var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1), book);

        book.AddClipping(clipping);
        book.AddClipping(clipping);

        Assert.Single(book.Clippings);
    }

    [Fact]
    public void Emoji_ReturnsBookEmoji()
    {
        var book = new Book("Author", "Title");

        Assert.Equal("📖", book.Emoji);
    }

    [Fact]
    public void LastSynchronized_GetterAndSetter_WorkAsExpected()
    {
        var book = new Book("Author", "Title");
        var expectedDateTime = new DateTime(2023, 1, 1);

        book.LastSynchronized = expectedDateTime;
        var retrievedDateTime = book.LastSynchronized;

        Assert.Equal(expectedDateTime, retrievedDateTime);
    }
}