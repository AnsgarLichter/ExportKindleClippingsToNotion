using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;
using Moq;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsParserFactory))]
public class ClippingsParserFactoryTest
{

    [Fact]
    public void ReturnsParserWithEnglishConfiguration()
    {
        var factory = new ClippingsParserFactory();
        
        Assert.IsType<ClippingsParserEnglish>(factory.GetByLanguage(SupportedLanguages.English));
    }
    
    [Fact]
    public void ReturnsParserWithGermanConfiguration()
    {
        var factory = new ClippingsParserFactory();
        
        Assert.IsType<ClippingsParserGerman>(factory.GetByLanguage(SupportedLanguages.German));
    }
}