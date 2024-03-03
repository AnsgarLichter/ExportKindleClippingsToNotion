namespace ExportKindleClippingsToNotion.Model;

public record Clipping(
    string Text,
    int StartPosition,
    int FinishPosition,
    int Page,
    DateTime HighlightDate,
    Book? Book);