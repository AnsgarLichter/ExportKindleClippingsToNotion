namespace ExportKindleClippingsToNotion.Model.Dto;

public record ClippingDto
{
    public ClippingDto(string text, int startPosition, int finishPosition, int page, DateTime highlightDate, string? author, string? title)
    {
        Text = text;
        StartPosition = startPosition;
        FinishPosition = finishPosition;
        Page = page;
        HighlightDate = highlightDate;
        Author = author;
        Title = title;
    }

    public string Text { get; }
    public int StartPosition { get; }
    public int FinishPosition { get; }
    public int Page { get; }
    public DateTime HighlightDate { get; }
    public string? Author { get; }
    public string? Title { get; }
}
