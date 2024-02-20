using ExportKindleClippingsToNotion.Model;
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
        var notionClient = new NotionClient("databaseId", notionClientMock);

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
        var notionClient = new NotionClient("databaseId", notionClientMock);
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
        var notionClient = new NotionClient("databaseId", notionClientMock);
        var book = new Book("Author", "Title");
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
        var notionClient = new NotionClient("databaseId", notionClientMock);
        var book = new Book("Author", "Title");
        var books = new List<Book> { book };

        var paginatedList = new PaginatedList<Page>()
        {
            Results =
            [
                new Page()
            ]
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
        //TODO: Mock PagesUpdateParametersBuilder.Create to be able to create the book

        await notionClient.Export(books);

        queryAsyncMock.MustHaveHappenedOnceExactly();
        createAsyncMock.MustNotHaveHappened();
        updateAsyncMock.MustHaveHappenedOnceExactly();
    }
}