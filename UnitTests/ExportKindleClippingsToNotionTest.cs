using ExportKindleClippingsToNotion;
using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Import.Metadata;
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
        var booksParserFactory = A.Fake<IBooksParserFactory>();
        var exporter = A.Fake<IExporter>();
        var language = A.Fake<IClippingsLanguage>();
        var clippingsParserFactory = A.Fake<IClippingsParserFactory>();
        var booksParser = A.Fake<IBooksParser>();
        A.CallTo(() => booksParserFactory.Create(A<IClippingsParser>._)).Returns(booksParser);

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
            new ExportKindleClippingsToNotion.ExportKindleClippingsToNotion(
                importer,
                booksParserFactory,
                exporter,
                language,
                clippingsParserFactory
            );

        await exportKindleClippingsToNotion.ExecuteAsync(clippingsPath);

        A.CallTo(() => language.Determine(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => clippingsParserFactory.GetByLanguage(A<SupportedLanguages>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => importer.ImportAsync(clippingsPath)).MustHaveHappenedOnceExactly();
        A.CallTo(() => booksParser.ParseAsync(clippings)).MustHaveHappenedOnceExactly();
        A.CallTo(() => exporter.ExportAsync(books)).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async void ExecuteAsync_ImportsEmptyFile()
    {
        var importer = A.Fake<IImporter>();
        var booksParserFactory = A.Fake<IBooksParserFactory>();
        var exporter = A.Fake<IExporter>();
        var language = A.Fake<IClippingsLanguage>();
        var clippingsParserFactory = A.Fake<IClippingsParserFactory>();
        var booksParser = A.Fake<IBooksParser>();
        A.CallTo(() => booksParserFactory.Create(A<IClippingsParser>._)).Returns(booksParser);

        const string clippingsPath = "path/to/clippings.txt";
        var clippings = Array.Empty<string>();
        var books = new List<Book>()
        {
            new Book("author1", "title1"),
            new Book("author2", "title2"),
            new Book("author3", "title3")
        };

        A.CallTo(() => importer.ImportAsync(clippingsPath)).Returns(Task.FromResult(clippings));
        A.CallTo(() => booksParser.ParseAsync(clippings)).Returns(books);
        
        var exportKindleClippingsToNotion =
            new ExportKindleClippingsToNotion.ExportKindleClippingsToNotion(
                importer,
                booksParserFactory,
                exporter,
                language,
                clippingsParserFactory
            );

        await exportKindleClippingsToNotion.ExecuteAsync(clippingsPath);

        A.CallTo(() => language.Determine(A<string>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => clippingsParserFactory.GetByLanguage(A<SupportedLanguages>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => importer.ImportAsync(clippingsPath)).MustHaveHappenedOnceExactly();
        A.CallTo(() => booksParser.ParseAsync(clippings)).MustNotHaveHappened();
        A.CallTo(() => exporter.ExportAsync(books)).MustNotHaveHappened();
    }
}