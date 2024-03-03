namespace ExportKindleClippingsToNotion.Model;

public record Book(string Author, string Title)
{
    
    private readonly string? _thumbnailUrl;
    public string? ThumbnailUrl
    {
        get => _thumbnailUrl;
        init => _thumbnailUrl = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value : null;
    }

    public string Emoji => "📖";
    public DateTimeOffset? LastSynchronized { get; set; }
    public HashSet<Clipping> Clippings { get; } = [];

    public void AddClipping(Clipping clipping)
    {
        Clippings.Add(clipping);
    }
}