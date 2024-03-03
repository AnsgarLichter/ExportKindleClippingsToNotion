namespace ExportKindleClippingsToNotion.Model.Dto;

public record ClippingDto
{
    public string Text { get; init; }
    public int StartPosition { get; init; }
    public int FinishPosition { get; init; }
    public int Page { get; init; }
    public DateTime HighlightDate { get; init; }
    public string? Author { get; init; }
    public string? Title { get; init; }
}
