using System.Globalization;

public class Config
{
    public string NotionAuthenticationToken { get; set; }
    public string NotionDatabaseId { get; set; }
    public CultureInfo Language { get; set; }

    public Config (string NotionAuthenticationToken, string NotionDatabaseId, CultureInfo Language) {
        this.NotionAuthenticationToken = NotionAuthenticationToken;
        this.NotionDatabaseId = NotionDatabaseId;
        this.Language = Language;
    }
}