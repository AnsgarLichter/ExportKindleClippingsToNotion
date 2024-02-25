namespace ExportKindleClippingsToNotion.Model;

public class Book(string author, string title)
{
    public string? Author { get; } = author;
    public string? Title { get; } = title;

    public string? Thumbnail { get; set; }

    public string Emoji => "📖";
    public DateTime? LastSynchronized { get; set; }
    public List<Clipping> Clippings { get; } = [];

    public void AddClipping(Clipping clipping)
    {
        if (Clippings.Contains(clipping))
        {
            return;
        }

        Clippings.Add(clipping);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var book = (Book)obj;
        return book.Author == this.Author && book.Title == this.Title;
    }

    public override int GetHashCode()
    {
        return (Author, Title).GetHashCode();
    }

    public override string ToString()
    {
        return $"{this.Title} ({this.Author})";
    }
}