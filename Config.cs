using System.Text.Json;

namespace ExportKindleClippingsToNotion;

public class Config
{
    public string NotionAuthenticationToken { get; set; }
    public string NotionDatabaseId { get; set; }
    public string Language { get; set; }

    public Config(string pathToConfig)
    {
        if (!File.Exists(pathToConfig))
        {
            throw new Exception($"The path '{pathToConfig}' to the config file isn't valid.");
        }
        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(pathToConfig));
        if (config is null
            || string.IsNullOrEmpty(config.NotionAuthenticationToken)
            || string.IsNullOrEmpty(config.NotionDatabaseId)
            || string.IsNullOrEmpty(config.Language))
        {
            throw new Exception(
                "Please provide an authentication token, your database id and your language in the config file");
        }
        
        NotionAuthenticationToken = config.NotionAuthenticationToken;
        NotionDatabaseId = config.NotionDatabaseId;
        Language = config.Language;
    }
}