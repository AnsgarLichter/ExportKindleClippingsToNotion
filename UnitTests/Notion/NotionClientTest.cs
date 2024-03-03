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
    public async Task GetDatabaseAsync_ReturnsDatabase()
    {
        var notionClientMock = A.Fake<INotionClient>();
        IPageBuilder pageBuilder = A.Fake<IPageBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pageBuilder);

        var retrieveAsyncMock = A.CallTo(() =>
            notionClientMock.Databases.RetrieveAsync("databaseId", A<CancellationToken>.Ignored));
        retrieveAsyncMock.Returns(Task.FromResult(new Database()));

        var result = await notionClient.GetDatabaseAsync();

        Assert.NotNull(result);
        retrieveAsyncMock.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task QueryAsync_ReturnsPaginatedListOfPages()
    {
        var notionClientMock = A.Fake<INotionClient>();
        IPageBuilder pageBuilder = A.Fake<IPageBuilder>();
        var notionClient = new NotionClient("databaseId", notionClientMock, pageBuilder);
        var book = new Book("Author", "Title");

        var queryAsyncMock = A.CallTo(() =>
            notionClientMock.Databases.QueryAsync("databaseId", A<DatabasesQueryParameters>.Ignored,
                A<CancellationToken>.Ignored));
        queryAsyncMock.Returns(Task.FromResult(new PaginatedList<Page>()));

        var result = await notionClient.QueryAsync(book);

        queryAsyncMock.MustHaveHappenedOnceExactly();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateAsync_SuccessfullyCreatesPage()
    {
        var databaseId = "testDatabaseId";
        var book = new Book("TestTitle", "TestAuthor");
        var pageId = "testPageId";

        var notionClientMock = A.Fake<INotionClient>();
        var pageBuilderMock = A.Fake<IPageBuilder>();

        var page = new Page { Id = pageId };

        A.CallTo(() => pageBuilderMock.Create(A<Book>.Ignored))
            .Returns(new PagesCreateParameters());
        A.CallTo(() =>
                notionClientMock.Pages.CreateAsync(A<PagesCreateParameters>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(page));

        var notionClient = new NotionClient(databaseId, notionClientMock, pageBuilderMock);

        await notionClient.CreateAsync(book);

        A.CallTo(() => pageBuilderMock.Create(A<Book>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() =>
                notionClientMock.Pages.CreateAsync(A<PagesCreateParameters>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateAsync_SuccessfullyUpdatesPage()
    {
        var databaseId = "testDatabaseId";
        var book = new Book("TestTitle", "TestAuthor");
        var pageId = "testPageId";
        var page = new Page { Id = pageId };

        var notionClientMock = A.Fake<INotionClient>();
        var pageBuilderMock = A.Fake<IPageBuilder>();

        A.CallTo(() => pageBuilderMock.Update(A<Page>.Ignored, A<Book>.Ignored))
            .Returns(new PagesUpdateParameters());
        A.CallTo(() =>
                notionClientMock.Pages.UpdateAsync(A<string>.Ignored, A<PagesUpdateParameters>.Ignored,
                    A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(page));

        A.CallTo(() => notionClientMock.Blocks.RetrieveChildrenAsync(A<string>.Ignored,
                A<BlocksRetrieveChildrenParameters>.Ignored,
                A<CancellationToken>.Ignored)
            )
            .Returns(Task.FromResult(new PaginatedList<IBlock>()
            {
                Results = new List<IBlock>()
                {
                    new TableBlock { Id = "testBlockId" }
                }
            }));
        A.CallTo(() => notionClientMock.Blocks.DeleteAsync(
            A<string>.Ignored,
            A<CancellationToken>.Ignored)
        );

        var notionClient = new NotionClient(databaseId, notionClientMock, pageBuilderMock);

        await notionClient.UpdateAsync(book, page);

        A.CallTo(() => pageBuilderMock.Update(A<Page>.Ignored, A<Book>.Ignored))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() =>
                notionClientMock.Pages.UpdateAsync(A<string>.Ignored, A<PagesUpdateParameters>.Ignored,
                    A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => notionClientMock.Blocks.RetrieveChildrenAsync(A<string>.Ignored,
                A<BlocksRetrieveChildrenParameters>.Ignored,
                A<CancellationToken>.Ignored)
            )
            .MustHaveHappenedOnceExactly();
    }
}