using ExportKindleClippingsToNotion;
using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Parser;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests;

[TestSubject(typeof(ExportKindleClippingsToNotion.ExportKindleClippingsToNotion))]
public class ExportKindleClippingsToNotionTest
{
    [Fact]
    public async void ExecuteAsync_ImportsParsesAndExports()
    {
        var importer = A.Fake<IImporter>();
        var booksParser = A.Fake<IBooksParser>();
        var exporter = A.Fake<IExporter>();

        const string clippingsPath = "path/to/clippings.txt";
        var clippings = new[] { "clipping1", "clipping2", "clipping3" };
        var books = new List<Book>()
        {
            new Book("author1", "title1"),
            new Book("author2", "title2"),
            new Book("author3", "title3")
        };

        A.CallTo(() => importer.ImportAsync(clippingsPath)).Returns(Task.FromResult(clippings));
        A.CallTo(() => booksParser.ParseAsync(clippings)).Returns(books);

        var exportKindleClippingsToNotion =
            new ExportKindleClippingsToNotion.ExportKindleClippingsToNotion(importer, booksParser, exporter);

        await exportKindleClippingsToNotion.ExecuteAsync(clippingsPath);
        
        A.CallTo(() => importer.ImportAsync(clippingsPath)).MustHaveHappenedOnceExactly();
        A.CallTo(() => booksParser.ParseAsync(clippings)).MustHaveHappenedOnceExactly();
        A.CallTo(() => exporter.ExportAsync(books)).MustHaveHappenedOnceExactly();
    }
}