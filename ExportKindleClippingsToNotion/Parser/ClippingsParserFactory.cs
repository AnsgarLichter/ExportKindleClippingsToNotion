namespace ExportKindleClippingsToNotion.Parser;

public class ClippingsParserFactory : IClippingsParserFactory
{
    public IClippingsParser GetByLanguage(SupportedLanguages languages)
    {
        switch (languages)
        {
            case SupportedLanguages.English:
                Console.WriteLine("Determined English language");
                return new ClippingsParserEnglish();
            case SupportedLanguages.German:
                Console.WriteLine("Determined German language");
                return new ClippingsParserGerman();
            case SupportedLanguages.Russian:
                Console.WriteLine("Determined Russian language");
                return new ClippingsParserRussian();
            default:
                throw new ArgumentOutOfRangeException(nameof(languages), languages, null);
        }
    }
}