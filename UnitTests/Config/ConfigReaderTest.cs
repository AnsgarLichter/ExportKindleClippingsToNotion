using System.IO.Abstractions;
using System.Text.Json;
using ExportKindleClippingsToNotion.Config;
using FakeItEasy;
using JetBrains.Annotations;

namespace UnitTests.Config;

[TestSubject(typeof(ConfigReader))]
public class ConfigReaderTest
{
    [Fact]
    public async Task Execute_ValidConfiguration_ReturnsConfig()
    {
        var fileSystem = A.Fake<IFileSystem>();
        const string pathToConfig = "valid_config.json";
        const string configContent =
            "{\"NotionAuthenticationToken\": \"token\", \"NotionDatabaseId\": \"id\"}";
        A.CallTo(() => fileSystem.File.Exists(pathToConfig)).Returns(true);
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            pathToConfig,
            A<CancellationToken>.Ignored
        )).Returns(Task.FromResult(configContent));

        var configReader = new ConfigReader(fileSystem);

        var config = await configReader.ExecuteAsync(pathToConfig);

        Assert.Equal("token", config.NotionAuthenticationToken);
        Assert.Equal("id", config.NotionDatabaseId);
    }

    [Fact]
    public async Task Execute_InvalidConfigurationFilePath_ThrowsException()
    {
        var fileSystem = A.Fake<IFileSystem>();
        const string pathToConfig = "invalid_config.json";
        A.CallTo(() => fileSystem.File.Exists(pathToConfig)).Returns(false);

        var configReader = new ConfigReader(fileSystem);

        await Assert.ThrowsAsync<Exception>(async () => await configReader.ExecuteAsync(pathToConfig));
    }

    [Theory]
    [InlineData("{\"NotionAuthenticationToken\": \"token\"}")]
    [InlineData("{\"NotionDatabaseId\": \"id\"}")]
    public async Task Execute_MissingField_ThrowsException(string configContent)
    {
        var fileSystem = A.Fake<IFileSystem>();
        const string pathToConfig = "missing_field_config.json";
        A.CallTo(() => fileSystem.File.Exists(pathToConfig)).Returns(true);
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            pathToConfig,
            A<CancellationToken>.Ignored
        )).Returns(Task.FromResult(configContent));

        var configReader = new ConfigReader(fileSystem);

        await Assert.ThrowsAsync<Exception>(async () => await configReader.ExecuteAsync(pathToConfig));
    }

    [Fact]
    public async Task Execute_InvalidConfigurationContent_ThrowsException()
    {
        var fileSystem = A.Fake<IFileSystem>();
        const string pathToConfig = "invalid_content_config.json";
        const string invalidContent = "invalid_json";

        A.CallTo(() => fileSystem.File.Exists(pathToConfig)).Returns(true);
        A.CallTo(() => fileSystem.File.ReadAllTextAsync(
            pathToConfig,
            A<CancellationToken>.Ignored
        )).Returns(Task.FromResult(invalidContent));

        var configReader = new ConfigReader(fileSystem);
        
        await Assert.ThrowsAsync<JsonException>(async () => await configReader.ExecuteAsync(pathToConfig));
    }
}