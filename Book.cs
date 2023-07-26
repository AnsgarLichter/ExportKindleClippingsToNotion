namespace ExportKindleClippingsToNotion;

public class Book
{
    public string? Author { get; set; }
    public string? Title { get; set; }

    public string? Thumbnail { get; set; }

    public string Emoji { get; set; } = "📖";
    public DateTime? LastSynchronized { get; set; }
    public int? Highlights { get; private set; }
    public List<Clipping> Clippings { get; set; } = new List<Clipping>();

    public Book(string author, string title)
    {
        this.Author = author;
        this.Title = title;
        this.Highlights = 0;
    }

    public void AddClipping(Clipping clipping)
    {
        if (Clippings.Contains(clipping))
        {
            return;
        }

        Clippings.Add(clipping);
        this.Highlights += 1;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Book book = (Book)obj;
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