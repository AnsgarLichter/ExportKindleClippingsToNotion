using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Model;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests.Export;

[TestSubject(typeof(Exporter))]
public class ExporterTest
{

    [Fact]
    public async Task Export_CallsExportMethodOfClient()
    {
        var client = A.Fake<IExportClient>();
        var exporter = new Exporter(client);
        var books = new List<Book> { new Book("author", "title") };
        
        await exporter.Export(books);
        
        A.CallTo(() => client.Export(books)).MustHaveHappenedOnceExactly();
    }
}