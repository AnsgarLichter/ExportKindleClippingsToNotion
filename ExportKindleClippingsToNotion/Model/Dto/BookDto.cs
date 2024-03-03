namespace ExportKindleClippingsToNotion.Model.Dto;

public class BookDto(string title, string author)
{
    public string Title { get; } = title;
    public string Author { get; } = author;
}