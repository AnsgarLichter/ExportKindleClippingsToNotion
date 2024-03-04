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
        var date = DateTime.Now;
        var book = new Book("Author", "Title")
        {
            ThumbnailUrl = "https://example.com/image.jpg",
            LastSynchronized = date
        };
        book.AddClipping(new Clipping("Text1", 1, 2, 1, date, book));
        book.AddClipping(new Clipping("Text2", 3, 4, 2, date, book));

        var result = _testSubject.Create(book);

        Assert.NotNull(result);
        Assert.NotNull(result.Parent);
        Assert.Equal("databaseId", (result.Parent as DatabaseParentInput).DatabaseId);
        Assert.NotNull(result.Icon);
        Assert.NotNull(result.Cover);
        Assert.NotNull(result.Properties);
        Assert.Equal(4, result.Properties.Count);
        AssertPagePropertySet(result.Properties, "Title", typeof(TitlePropertyValue), "Title");
        AssertPagePropertySet(result.Properties, "Author", typeof(RichTextPropertyValue), "Author");
        AssertPagePropertySet(result.Properties, "Highlights", typeof(NumberPropertyValue), "2");
        AssertPagePropertySet(result.Properties, "Last Synchronized", typeof(DatePropertyValue),
            date.ToString());
        Assert.Equal(book.Emoji, (result.Icon as EmojiObject).Emoji);
        Assert.Equal(book.ThumbnailUrl, (result.Cover as ExternalFile).External.Url);
        Assert.NotNull(result.Properties);
        Assert.NotNull(result.Children);
    }

    [Fact]
    public void CreateClippingsTable_ValidBook_ReturnsTableBlock()
    {
        var date = DateTime.Now;
        var book = new Book("Author", "Title")
        {
            ThumbnailUrl = "https://example.com/image.jpg",
            LastSynchronized = date
        };
        book.AddClipping(new Clipping("Text1", 1, 2, 1, date, book));
        book.AddClipping(new Clipping("Text2", 3, 4, 2, date, book));

        var result = _testSubject.CreateClippingsTable(book);

        Assert.NotNull(result);
        Assert.IsType<TableBlock>(result);

        var tableBlock = (TableBlock)result;
        Assert.NotNull(tableBlock.Table);
        Assert.True(tableBlock.Table.HasColumnHeader);
        Assert.False(tableBlock.Table.HasRowHeader);
        Assert.Equal(4, tableBlock.Table.TableWidth);
        Assert.NotEmpty(tableBlock.Table.Children);

        var tableRows = tableBlock.Table.Children.ToList();
        Assert.Equal(3, tableRows.Count);

        //First Row
        var firstRow = tableRows[0];
        Assert.NotNull(firstRow.TableRow);
        Assert.NotNull(firstRow.TableRow.Cells);


        var cells = firstRow.TableRow.Cells.ToList();

        var firstCellContents = cells[0].ToList();
        Assert.Single(firstCellContents);
        var firstCellText = firstCellContents[0].Text.Content;
        Assert.Equal("Clipping", firstCellText);

        var secondCellContents = cells[1].ToList();
        Assert.Single(secondCellContents);
        var secondCellText = secondCellContents[0].Text.Content;
        Assert.Equal("Page", secondCellText);

        var thirdCellContents = cells[2].ToList();
        Assert.Single(thirdCellContents);
        var thirdCellText = thirdCellContents[0].Text.Content;
        Assert.Equal("Start Position", thirdCellText);

        var fourthCellContents = cells[3].ToList();
        Assert.Single(fourthCellContents);
        var fourthCellText = fourthCellContents[0].Text.Content;
        Assert.Equal("Finish Position", fourthCellText);

        var secondRow = tableRows[1];
        Assert.NotNull(secondRow.TableRow);
        Assert.NotNull(secondRow.TableRow.Cells);

        var secondRowCells = secondRow.TableRow.Cells.ToList();
        Assert.Equal(4, secondRowCells.Count);

        var secondRowFirstCellText = secondRowCells[0].ToList()[0].Text.Content;
        Assert.Equal("Text1", secondRowFirstCellText);

        var secondRowSecondCellText = secondRowCells[1].ToList()[0].Text.Content;
        Assert.Equal("1", secondRowSecondCellText);

        var secondRowThirdCellText = secondRowCells[2].ToList()[0].Text.Content;
        Assert.Equal("1", secondRowThirdCellText);

        var secondRowFourthCellText = secondRowCells[3].ToList()[0].Text.Content;
        Assert.Equal("2", secondRowFourthCellText);

        var thirdRow = tableRows[2];
        Assert.NotNull(thirdRow.TableRow);
        Assert.NotNull(thirdRow.TableRow.Cells);

        var thirdRowCells = thirdRow.TableRow.Cells.ToList();
        Assert.Equal(4, thirdRowCells.Count);

        var thirdRowFirstCellText = thirdRowCells[0].ToList()[0].Text.Content;
        Assert.Equal("Text2", thirdRowFirstCellText);

        var thirdRowSecondCellText = thirdRowCells[1].ToList()[0].Text.Content;
        Assert.Equal("2", thirdRowSecondCellText);

        var thirdRowThirdCellText = thirdRowCells[2].ToList()[0].Text.Content;
        Assert.Equal("3", thirdRowThirdCellText);

        var thirdRowFourthCellText = thirdRowCells[3].ToList()[0].Text.Content;
        Assert.Equal("4", thirdRowFourthCellText);
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
        var date = DateTime.Now;
        var book = new Book("Author", "Title")
        {
            ThumbnailUrl = "https://example.com/image.jpg",
            LastSynchronized = date
        };
        book.AddClipping(new Clipping("Text1", 1, 2, 1, date, book));
        book.AddClipping(new Clipping("Text2", 3, 4, 2, date, book));

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

    private void AssertPagePropertySet(IDictionary<string, PropertyValue> properties, string key, Type
        propertyValueClazz, string expectedValue)
    {
        PropertyValue value;
        Assert.True(properties.TryGetValue(key, out value), $"property {key} not found in dictionary.");
        Assert.NotNull(value);

        var actualProperty = Convert.ChangeType(value, propertyValueClazz);
        Assert.NotNull(actualProperty);

        switch (propertyValueClazz.Name)
        {
            case nameof(TitlePropertyValue):
                var titleProperty = propertyValueClazz.GetProperty("Title");
                Assert.NotNull(titleProperty);
                var titleValue = titleProperty.GetValue(actualProperty);
                Assert.NotNull(titleValue);

                if (titleValue is List<RichTextBase> richTextList && richTextList.Count > 0)
                {
                    var firstRichText = richTextList[0];
                    if (firstRichText is RichTextText richTextText)
                    {
                        Assert.Equal(expectedValue, richTextText.Text.Content);
                    }
                    else
                    {
                        Assert.True(false, "Invalid property type");
                    }
                }
                else
                {
                    Assert.True(false, "Invalid property type");
                }

                break;
            case nameof(RichTextPropertyValue):
                var richtTextProperty = propertyValueClazz.GetProperty("RichText");
                Assert.NotNull(richtTextProperty);
                var richTextValue = richtTextProperty.GetValue(actualProperty);
                Assert.NotNull(richTextValue);

                if (richTextValue is List<RichTextBase> richTextList2 && richTextList2.Count > 0)
                {
                    var firstRichText = richTextList2[0];
                    if (firstRichText is RichTextText richTextText)
                    {
                        Assert.Equal(expectedValue, richTextText.Text.Content);
                    }
                    else
                    {
                        Assert.True(false, "Invalid property type");
                    }
                }
                else
                {
                    Assert.True(false, "Invalid property type");
                }

                break;
            case nameof(NumberPropertyValue):
                var numberProperty = propertyValueClazz.GetProperty("Number");
                Assert.NotNull(numberProperty);
                Assert.Equal(expectedValue, numberProperty.GetValue(actualProperty)?.ToString());
                break;
            case nameof(DatePropertyValue):
                var dateProperty = propertyValueClazz.GetProperty("Date");
                Assert.NotNull(dateProperty);
                var dateValue = dateProperty.GetValue(actualProperty);
                if (dateValue is Date startDateValue)
                {
                    Assert.Equal(expectedValue, startDateValue.Start.ToString());
                }
                else
                {
                    Assert.True(false, "Invalid property type");
                }

                break;
            default:
                Assert.True(false, "Unexpected property Type");
                break;
        }
    }
}