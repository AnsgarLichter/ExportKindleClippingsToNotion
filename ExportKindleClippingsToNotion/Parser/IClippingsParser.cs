using ExportKindleClippingsToNotion.Model.Dto;

namespace ExportKindleClippingsToNotion.Parser;

public interface IClippingsParser
{
    public Task<ClippingDto?> ParseAsync(string clipping);
}