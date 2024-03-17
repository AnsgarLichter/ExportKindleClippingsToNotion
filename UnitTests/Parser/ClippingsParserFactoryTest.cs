using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;
using Moq;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsParserFactory))]
public class ClippingsParserFactoryTest
{

    [Fact]
    public void ReturnsEnglishParserForEnglishLanguage()
    {
        var factory = new ClippingsParserFactory();
        
        Assert.IsType<ClippingsParserEnglish>(factory.GetByLanguage(SupportedLanguages.English));
    }
    
    [Fact]
    public void ReturnsGermanParserForGermanLanguage()
    {
        var factory = new ClippingsParserFactory();
        
        Assert.IsType<ClippingsParserGerman>(factory.GetByLanguage(SupportedLanguages.German));
    }
    
    [Fact]
    public void ReturnsRussianParserForRussianLanguage()
    {
        var factory = new ClippingsParserFactory();
        
        Assert.IsType<ClippingsParserRussian>(factory.GetByLanguage(SupportedLanguages.Russian));
    }

    [Fact]
    public void ThrowsExceptionForUnsupportedLanguage()
    {
        var factory = new ClippingsParserFactory();
        const SupportedLanguages unsupportedLanguage = (SupportedLanguages)100;

        Assert.Throws<ArgumentOutOfRangeException>(() => factory.GetByLanguage(unsupportedLanguage));
    }
}