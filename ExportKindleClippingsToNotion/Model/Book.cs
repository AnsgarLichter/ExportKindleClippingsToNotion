namespace ExportKindleClippingsToNotion.Model;

public record Book(string Author, string Title)
{
    public string? Thumbnail { get; set; }

    public string Emoji => "📖";
    public DateTime? LastSynchronized { get; set; }
    public HashSet<Clipping> Clippings { get; } = [];

    public void AddClipping(Clipping clipping)
    {
        Clippings.Add(clipping);
    }
}