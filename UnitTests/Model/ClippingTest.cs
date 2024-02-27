using ExportKindleClippingsToNotion.Model;
using JetBrains.Annotations;

namespace UnitTests.Model;

[TestSubject(typeof(Clipping))]
public class ClippingTest
{
    [Fact]
    public void TestClipping()
    {
        var clipping = new Clipping("text", 1, 2, 3, DateTime.Now);
        Assert.Equal("text", clipping.Text);
        Assert.Equal(1, clipping.StartPosition);
        Assert.Equal(2, clipping.FinishPosition);
        Assert.Equal(3, clipping.Page);
        Assert.Equal(DateTime.Now.Date, clipping.HighlightDate.Date);
    }
}