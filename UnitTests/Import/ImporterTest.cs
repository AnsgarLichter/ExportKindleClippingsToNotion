using ExportKindleClippingsToNotion.Import;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests.Import;

[TestSubject(typeof(Importer))]
public class ImporterTest
{

    [Fact]
    public async Task Import_CallsImportMethodOfClient()
    {
        var client = A.Fake<IImportClient>();
        var importer = new Importer(client);
        const string pathToClippings = "path/to/clippings.txt";
        var expectedClippings = new string[] { "Clipping 1", "Clipping 2" };
        A.CallTo(() => client.ImportAsync(pathToClippings)).Returns(expectedClippings);
        
        var result = await importer.ImportAsync(pathToClippings);
        
        Assert.Equal(expectedClippings, result);
        A.CallTo(() => client.ImportAsync(pathToClippings)).MustHaveHappenedOnceExactly();
    }
}