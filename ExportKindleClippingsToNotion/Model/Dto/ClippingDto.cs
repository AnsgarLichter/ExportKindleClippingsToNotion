namespace ExportKindleClippingsToNotion.Model.Dto;

public class ClippingDto
{
    public ClippingDto(Clipping? clipping, string? author, string? title)
    {
        Clipping = clipping;
        Author = author;
        Title = title;
    }

    public Clipping? Clipping { get;  }
    public string? Author { get; }
    public string? Title { get; }
}