using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Model;
using FakeItEasy;
using JetBrains.Annotations;
using Notion.Client;

namespace UnitTests.Export;

[TestSubject(typeof(Exporter))]
public class ExporterTest
{
    private readonly IExportClient _clientMock;
    private readonly Exporter _testSubject;

    public ExporterTest()
    {
        _clientMock = A.Fake<IExportClient>();
        _testSubject = new Exporter(_clientMock);
    }

    [Fact]
    public async Task ExportAsync_NoPages_CreateCalled()
    {
        var books = new List<Book> { new Book("Title1", "Author1") };
        A.CallTo(() => _clientMock.QueryAsync(A<Book>._))
            .Returns(Task.FromResult(new PaginatedList<Page>() { Results = [] }));

        await _testSubject.ExportAsync(books);

        A.CallTo(() => _clientMock.CreateAsync(A<Book>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _clientMock.UpdateAsync(A<Book>._, A<Page>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExportAsync_OnePage_UpdateCalled()
    {
        var books = new List<Book> { new Book("Title1", "Author1") };
        A.CallTo(() => _clientMock.QueryAsync(A<Book>._))
            .Returns(Task.FromResult(new PaginatedList<Page>() { Results = [new Page()] }));

        await _testSubject.ExportAsync(books);

        A.CallTo(() => _clientMock.CreateAsync(A<Book>._)).MustNotHaveHappened();
        A.CallTo(() => _clientMock.UpdateAsync(A<Book>._, A<Page>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExportAsync_MultiplePages_UpdateCalledWithFirstPage()
    {
        var books = new List<Book> { new Book("Title1", "Author1") };
        var page = new Page
        {
            Id = "FirstPage",
        };
        A.CallTo(() => _clientMock.QueryAsync(A<Book>._))
            .Returns(Task.FromResult(new PaginatedList<Page>()
            {
                Results =
                [
                    page,
                    new Page()
                    {
                        Id = "SecondPage"
                    }
                ]
            }));

        await _testSubject.ExportAsync(books);

        A.CallTo(() => _clientMock.CreateAsync(A<Book>._)).MustNotHaveHappened();
        A.CallTo(() => _clientMock.UpdateAsync(A<Book>._, page)).MustHaveHappenedOnceExactly();
    }
}