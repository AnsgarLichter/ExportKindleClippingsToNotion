using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Notion.Utils;
using FakeItEasy;
using JetBrains.Annotations;
using Notion.Client;
using NotionClient = ExportKindleClippingsToNotion.Notion.NotionClient;

namespace UnitTests.Notion;

[TestSubject(typeof(NotionClient))]
public class NotionClientTest
{
    [Fact]
    public async Task GetDatabase_ReturnsDatabase()
    {
        var notionClientMock = A.Fake<INotionClient>();
        var pagesUpdateParametersBuilderMock = A.Fake<IPagesUpdateParametersBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pagesUpdateParametersBuilderMock);

        var retrieveAsyncMock = A.CallTo(() =>
            notionClientMock.Databases.RetrieveAsync("databaseId", A<CancellationToken>.Ignored));
        retrieveAsyncMock.Returns(Task.FromResult(new Database()));

        var result = await notionClient.GetDatabase();

        Assert.NotNull(result);
        retrieveAsyncMock.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Query_ReturnsPaginatedListOfPages()
    {
        var notionClientMock = A.Fake<INotionClient>();
        var pagesUpdateParametersBuilderMock = A.Fake<IPagesUpdateParametersBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pagesUpdateParametersBuilderMock);
        var book = new Book("Author", "Title");

        var queryAsyncMock = A.CallTo(() =>
            notionClientMock.Databases.QueryAsync("databaseId", A<DatabasesQueryParameters>.Ignored,
                A<CancellationToken>.Ignored));
        queryAsyncMock.Returns(Task.FromResult(new PaginatedList<Page>()));

        var result = await notionClient.Query(book);

        queryAsyncMock.MustHaveHappenedOnceExactly();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Export_CreatesPageForBooks()
    {
        var notionClientMock = A.Fake<INotionClient>();
        var pagesUpdateParametersBuilderMock = A.Fake<IPagesUpdateParametersBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pagesUpdateParametersBuilderMock);
        var book = new Book("Author", "Title")
        {
            Clippings = { new Clipping("text", 1, 2, 3, DateTime.Now) }
        };
        var books = new List<Book> { book };

        var paginatedList = new PaginatedList<Page>()
        {
            Results = []
        };
        var queryAsyncMock = A.CallTo(() =>
            notionClientMock.Databases.QueryAsync("databaseId", A<DatabasesQueryParameters>.Ignored,
                A<CancellationToken>.Ignored));
        queryAsyncMock.Returns(Task.FromResult(paginatedList));
        var createAsyncMock = A.CallTo(() =>
            notionClientMock.Pages.CreateAsync(A<PagesCreateParameters>.Ignored, A<CancellationToken>.Ignored));
        createAsyncMock.Returns(Task.FromResult(new Page()));
        var updateAsyncMock = A.CallTo(() =>
            notionClientMock.Pages.UpdateAsync(A<string>.Ignored, A<PagesUpdateParameters>.Ignored,
                A<CancellationToken>.Ignored));
        updateAsyncMock.Returns(Task.FromResult(new Page()));

        await notionClient.Export(books);

        queryAsyncMock.MustHaveHappenedOnceExactly();
        createAsyncMock.MustHaveHappenedOnceExactly();
        updateAsyncMock.MustNotHaveHappened();
    }

    [Fact]
    public async Task Export_UpdatesPageForBooks()
    {
        var notionClientMock = A.Fake<INotionClient>();
        var pagesUpdateParametersBuilderMock = A.Fake<IPagesUpdateParametersBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pagesUpdateParametersBuilderMock);
        var book = new Book("Author", "Title");
        var books = new List<Book> { book };

        var paginatedList = new PaginatedList<Page>()
        {
            Results =
            [
                new Page()
                {
                    Properties = new Dictionary<string, PropertyValue>()
                    {
                        { "A", new TitlePropertyValue() }
                    }
                }
            ]
        };
        A.CallTo(() =>
            notionClientMock.Databases.QueryAsync(
                "databaseId",
                A<DatabasesQueryParameters>.Ignored,
                A<CancellationToken>.Ignored)
        ).Returns(Task.FromResult(paginatedList));
        A.CallTo(() =>
            notionClientMock.Pages.CreateAsync(A<PagesCreateParameters>.Ignored, A<CancellationToken>.Ignored)
        ).Throws(new Exception("Shouldn't have happened"));

        A.CallTo(() =>
            pagesUpdateParametersBuilderMock.WithProperty(
                "LastEdited",
                A<PropertyValue>.Ignored)
        ).Throws(new Exception("Shouldn't have happened"));
        A.CallTo(() => pagesUpdateParametersBuilderMock.Build()).Returns(new PagesUpdateParameters());

        var updateAsyncMock = A.CallTo(() =>
            notionClientMock.Pages.UpdateAsync(A<string>.Ignored, A<PagesUpdateParameters>.Ignored,
                A<CancellationToken>.Ignored));
        updateAsyncMock.Returns(Task.FromResult(new Page
        {
            Id = "ID"
        }));

        A.CallTo(() =>
            notionClientMock.Blocks.RetrieveChildrenAsync(
                A<string>.Ignored,
                A<BlocksRetrieveChildrenParameters>.Ignored,
                A<CancellationToken>.Ignored
            )).Returns(Task.FromResult<PaginatedList<IBlock>>(new PaginatedList<IBlock>()
        {
            Results = new List<IBlock>()
            {
                A.Fake<IBlock>()
            }
        }));

        A.CallTo(() =>
            notionClientMock.Blocks.DeleteAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored
            )).Returns(Task.FromResult<HttpResponseMessage>(new HttpResponseMessage()));

        A.CallTo(() =>
            notionClientMock.Blocks.AppendChildrenAsync(
                A<string>.Ignored,
                A<BlocksAppendChildrenParameters>.Ignored,
                A<CancellationToken>.Ignored
            )).Returns(Task.FromResult(new PaginatedList<IBlock>()));

        await notionClient.Export(books);

        updateAsyncMock.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Export_UpdatesPageForBooksFails()
    {
        var notionClientMock = A.Fake<INotionClient>();
        var pagesUpdateParametersBuilderMock = A.Fake<IPagesUpdateParametersBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pagesUpdateParametersBuilderMock);
        var book = new Book("Author", "Title");
        var books = new List<Book> { book };

        var paginatedList = new PaginatedList<Page>()
        {
            Results =
            [
                new Page()
                {
                    Properties = new Dictionary<string, PropertyValue>()
                }
            ]
        };
        A.CallTo(() =>
            notionClientMock.Databases.QueryAsync(
                "databaseId",
                A<DatabasesQueryParameters>.Ignored,
                A<CancellationToken>.Ignored)
        ).Returns(Task.FromResult(paginatedList));
        A.CallTo(() =>
            notionClientMock.Pages.CreateAsync(A<PagesCreateParameters>.Ignored, A<CancellationToken>.Ignored)
        ).Throws(new Exception("Shouldn't have happened"));

        A.CallTo(() =>
            pagesUpdateParametersBuilderMock.WithProperty(
                "LastEdited",
                A<PropertyValue>.Ignored)
        ).Throws(new Exception("Shouldn't have happened"));
        A.CallTo(() => pagesUpdateParametersBuilderMock.Build()).Returns(new PagesUpdateParameters());

        var updateAsyncMock = A.CallTo(() =>
            notionClientMock.Pages.UpdateAsync(A<string>.Ignored, A<PagesUpdateParameters>.Ignored,
                A<CancellationToken>.Ignored));
        updateAsyncMock.Returns(Task.FromResult(new Page
        {
            Id = null
        }));

        A.CallTo(() =>
            notionClientMock.Blocks.DeleteAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored
            )).Returns(Task.FromResult<HttpResponseMessage>(new HttpResponseMessage()));

        A.CallTo(() =>
            notionClientMock.Blocks.AppendChildrenAsync(
                A<string>.Ignored,
                A<BlocksAppendChildrenParameters>.Ignored,
                A<CancellationToken>.Ignored
            )).Returns(Task.FromResult(new PaginatedList<IBlock>()));

        await Assert.ThrowsAsync<Exception>(() => notionClient.Export(books));

        updateAsyncMock.MustHaveHappenedOnceExactly();
    }
}