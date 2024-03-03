using ExportKindleClippingsToNotion.Model.Dto;

namespace ExportKindleClippingsToNotion.Parser;

public interface IClippingsParser
{
    public ClippingDto? Parse(string clipping);
}