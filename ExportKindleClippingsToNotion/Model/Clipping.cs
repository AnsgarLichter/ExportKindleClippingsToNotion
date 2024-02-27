namespace ExportKindleClippingsToNotion.Model;

public record Clipping
{
    public string? Text { get; }
    public int StartPosition { get; }
    public int FinishPosition { get; }
    public int Page { get; }
    public DateTime HighlightDate { get; }
    public Book? Book { get; set; }

    public Clipping(string text, int startPosition, int finishPosition, int page, DateTime highlightDate)
    {
        Text = text;
        StartPosition = startPosition;
        FinishPosition = finishPosition;
        Page = page;
        HighlightDate = highlightDate;
    }
}