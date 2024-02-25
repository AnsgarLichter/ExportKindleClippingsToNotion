﻿using System.IO.Abstractions;

namespace ExportKindleClippingsToNotion.Config;

public class ConfigReader(IFileSystem fileSystem): IConfigReader
{
    public async Task<Config> ExecuteAsync(string pathToConfig)
    {
        if (!fileSystem.File.Exists(pathToConfig))
        {
            throw new Exception($"The path '{pathToConfig}' to the config file isn't valid.");
        }

        var content = await fileSystem.File.ReadAllTextAsync(pathToConfig);
        var config = System.Text.Json.JsonSerializer.Deserialize<Config>(content);
        if (config is null
            || string.IsNullOrEmpty(config.NotionAuthenticationToken)
            || string.IsNullOrEmpty(config.NotionDatabaseId)
            || string.IsNullOrEmpty(config.Language))
        {
            throw new Exception(
                "Please provide an authentication token, your database id and your language in the config file");
        }

        return config;
    }
}