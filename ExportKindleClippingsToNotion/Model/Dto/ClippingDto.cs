namespace ExportKindleClippingsToNotion.Model.Dto;

// TODO: The DTO shouldn't contain an instance of the model itself
public record ClippingDto(string Text, int StartPosition, int FinishPosition, int Page, DateTime HighlightDate, string? Author, string? Title);