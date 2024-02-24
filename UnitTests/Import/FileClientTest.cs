using System.IO.Abstractions;
using ExportKindleClippingsToNotion.Import;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests.Import;

[TestSubject(typeof(FileClient))]
public class FileClientTest
{
    [Fact]
    public async Task Import_ReturnsFormattedClippings()
    {
        const string pathToClippings = "path/to/clippings.txt";
        const string clippingsText =
            "Title 1\nMetadata 1\n\nText1\n==========\r\nTitle2\nMetadata2\n\nText2\n==========\r\nTitle3\nMetadata3\n\nText3";
        var fileSystem = A.Fake<IFileSystem>();
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            pathToClippings,
            A<CancellationToken>.Ignored
        )).Returns(Task.FromResult(clippingsText));
        var fileClient = new FileClient(fileSystem);

        var result = await fileClient.Import(pathToClippings);

        Assert.Equal(["Title 1\nMetadata 1\n\nText1\n", "Title2\nMetadata2\n\nText2\n", "Title3\nMetadata3\n\nText3"],
            result);
    }

    [Fact]
    public async Task Import_WithEmptyFile_ReturnsEmptyArray()
    {
        const string pathToClippings = "path/to/empty.txt";
        var clippingsText = string.Empty;
        var fileSystem = A.Fake<IFileSystem>();
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            pathToClippings,
            A<CancellationToken>.Ignored
        )).Returns(Task.FromResult(clippingsText));
        var fileClient = new FileClient(fileSystem);

        var result = await fileClient.Import(pathToClippings);

        Assert.Equal([""], result);
    }

    [Fact]
    public async Task Import_WithNullPath_ThrowsArgumentNullException()
    {
        var fileSystem = A.Fake<IFileSystem>();
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            A<string>.Ignored,
            A<CancellationToken>.Ignored
        )).Throws<ArgumentNullException>();
        var fileClient = new FileClient(fileSystem);

        await Assert.ThrowsAsync<ArgumentNullException>(() => fileClient.Import(null));
    }

    [Fact]
    public async Task Import_WithNonexistentFile_ThrowsFileNotFoundException()
    {
        const string pathToClippings = "nonexistent/file.txt";
        var fileSystem = A.Fake<IFileSystem>();
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            A<string>.Ignored,
            A<CancellationToken>.Ignored
        )).Throws<FileNotFoundException>();
        var fileClient = new FileClient(fileSystem);

        await Assert.ThrowsAsync<FileNotFoundException>(() => fileClient.Import(pathToClippings));
    }

    [Fact]
    public async Task Import_WithMalformedClippings_ReturnsArrayOfClippings()
    {
        const string pathToClippings = "path/to/clippings.txt";
        const string clippingsText = "Clipping 1==========\r\nClipping 2==========\r\n==========";
        var fileSystem = A.Fake<IFileSystem>();
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            pathToClippings,
            A<CancellationToken>.Ignored
        )).Returns(Task.FromResult(clippingsText));
        var fileClient = new FileClient(fileSystem);

        var result = await fileClient.Import(pathToClippings);

        Assert.Equal(["Clipping 1", "Clipping 2", "=========="], result);
    }
}