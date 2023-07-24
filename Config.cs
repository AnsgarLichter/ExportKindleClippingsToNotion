namespace ExportKindleClippingsToNotion;

public class Config
{
    public string NotionAuthenticationToken { get; set; }
    public string NotionDatabaseId { get; set; }
    public string Language { get; set; }

    public Config(string notionAuthenticationToken, string notionDatabaseId, string language)
    {
        this.NotionAuthenticationToken = notionAuthenticationToken;
        this.NotionDatabaseId = notionDatabaseId;
        this.Language = language;
    }
}