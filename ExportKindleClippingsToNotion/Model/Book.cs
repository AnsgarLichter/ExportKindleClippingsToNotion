namespace ExportKindleClippingsToNotion.Model;

public record Book(string Author, string Title)
{
    // TODO: Add validation of thumbnail in setter
    
    private string? _thumbnailUrl;
    public string? ThumbnailUrl
    {
        get => _thumbnailUrl;
        set => _thumbnailUrl = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value : null;
    }

    public string Emoji => "📖";
    public DateTime? LastSynchronized { get; set; }
    public HashSet<Clipping> Clippings { get; } = [];

    public void AddClipping(Clipping clipping)
    {
        Clippings.Add(clipping);
    }
}