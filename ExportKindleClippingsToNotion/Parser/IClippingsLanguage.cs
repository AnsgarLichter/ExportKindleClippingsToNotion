namespace ExportKindleClippingsToNotion.Parser;

public interface IClippingsLanguage
{
    SupportedLanguages Determine(string clipping);
}