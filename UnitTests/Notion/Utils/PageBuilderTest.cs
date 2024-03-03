using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Notion.Utils;
using FakeItEasy;
using JetBrains.Annotations;
using Notion.Client;

namespace UnitTests.Notion.Utils;

[TestSubject(typeof(PageBuilder))]
public class PageBuilderTest
{
    private readonly string _databaseId;
    private readonly IPagesUpdateParametersBuilder _updateParametersBuilder;
    private IPageBuilder _testSubject;

    public PageBuilderTest()
    {
        _databaseId = "databaseId";
        _updateParametersBuilder = A.Fake<IPagesUpdateParametersBuilder>();
        _testSubject = new PageBuilder(_databaseId, _updateParametersBuilder);
    }

    [Fact]
    public void Create_ValidBook_ReturnsPagesCreateParameters()
    {
        var book = new Book("Author", "Title")
        {
            ThumbnailUrl = "https://example.com/image.jpg"
        };
        book.AddClipping(new Clipping("Text1", 1, 2, 1, DateTime.Now, book));
        book.AddClipping(new Clipping("Text2", 3, 4, 2, DateTime.Now, book));

        var result = _testSubject.Create(book);

        Assert.NotNull(result);
        Assert.NotNull(result.Parent);
        Assert.Equal("databaseId", (result.Parent as DatabaseParentInput).DatabaseId);
        Assert.NotNull(result.Icon);
        Assert.NotNull(result.Cover);
        Assert.NotNull(result.Properties);
        Assert.Equal(4, result.Properties.Count);
        AssertPagePropertySet(result.Properties, "Title");
        AssertPagePropertySet(result.Properties, "Author");
        AssertPagePropertySet(result.Properties, "Highlights");
        AssertPagePropertySet(result.Properties, "Last Synchronized");
        Assert.NotNull(result.Icon);
        Assert.NotNull(result.Cover);
        Assert.NotNull(result.Properties);
    }

    [Fact]
    public void CreateClippingsTable_ValidBook_ReturnsTableBlock()
    {
        var book = new Book("Author", "Title")
        {
            ThumbnailUrl = "https://example.com/image.jpg"
        };
        book.AddClipping(new Clipping("Text1", 1, 2, 1, DateTime.Now, book));
        book.AddClipping(new Clipping("Text2", 3, 4, 2, DateTime.Now, book));

        var result = _testSubject.CreateClippingsTable(book);

        Assert.NotNull(result);
        Assert.IsType<TableBlock>(result);

        var tableBlock = (TableBlock)result;
        Assert.NotNull(tableBlock.Table);
        Assert.True(tableBlock.Table.HasColumnHeader);
        Assert.False(tableBlock.Table.HasRowHeader);
        Assert.Equal(4, tableBlock.Table.TableWidth);
        Assert.NotEmpty(tableBlock.Table.Children);

        var rowEnumerator = tableBlock.Table.Children.GetEnumerator();
        Assert.True(rowEnumerator.MoveNext(), "No table rows found in the table.");
        Assert.True(rowEnumerator.MoveNext(), "No table rows found in the table.");
        Assert.True(rowEnumerator.MoveNext(), "No table rows found in the table.");
        Assert.False(rowEnumerator.MoveNext(), "Too many table rows found in the table.");
    }

    [Fact]
    public void Update_ValidPageAndBook_ReturnsPagesUpdateParameters()
    {
        var page = new Page()
        {
            Properties = new Dictionary<string, PropertyValue>
            {
                { "Title", new TitlePropertyValue() },
                { "Author", new RichTextPropertyValue() },
                { "Last Edited", new RichTextPropertyValue() }
            }
        };
        var book = new Book("Author", "Title")
        {
            ThumbnailUrl = "https://example.com/image.jpg"
        };

        var WithPropertyAuthor = A.CallTo(() =>
            _updateParametersBuilder.WithProperty("Author", A<RichTextPropertyValue>.Ignored));
        var WithPropertyTitle = A.CallTo(() =>
            _updateParametersBuilder.WithProperty("Title", A<TitlePropertyValue>.Ignored));
        var WithPropertyHighlights = A.CallTo(() =>
            _updateParametersBuilder.WithProperty("Highlights", A<NumberPropertyValue>.Ignored));
        var WithPropertyLastSynced = A.CallTo(() =>
            _updateParametersBuilder.WithProperty("Last Synchronized", A<DatePropertyValue>.Ignored));
        var WithPropertyLastEdited = A.CallTo(() =>
            _updateParametersBuilder.WithProperty("Last Edited", A<DatePropertyValue>.Ignored));

        WithPropertyAuthor.Returns(_updateParametersBuilder);
        WithPropertyTitle.Returns(_updateParametersBuilder);
        WithPropertyHighlights.Returns(_updateParametersBuilder);
        WithPropertyLastSynced.Returns(_updateParametersBuilder);

        var result = _testSubject.Update(page, book);

        Assert.NotNull(result);
        WithPropertyAuthor.MustHaveHappened();
        WithPropertyTitle.MustHaveHappened();
        WithPropertyHighlights.MustHaveHappenedOnceExactly();
        WithPropertyLastSynced.MustHaveHappenedOnceExactly();
        WithPropertyLastEdited.MustNotHaveHappened();
    }

    private void AssertPagePropertySet(IDictionary<string, PropertyValue> properties, string key)
    {
        PropertyValue value;
        Assert.True(properties.TryGetValue(key, out value), "Title property not found in dictionary.");
    }
}