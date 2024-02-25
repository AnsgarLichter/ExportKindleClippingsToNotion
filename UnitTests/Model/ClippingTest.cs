using ExportKindleClippingsToNotion.Model;
using JetBrains.Annotations;

namespace UnitTests.Model;

[TestSubject(typeof(Clipping))]
public class ClippingTest
{

    [Fact]
    public void GetHashCode_ReturnsSameValueForSameProperties()
    {
        var obj1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
        var obj2 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
        
        var hashCode1 = obj1.GetHashCode();
        var hashCode2 = obj2.GetHashCode();
        
        Assert.Equal(hashCode1, hashCode2);
    }
    
    [Fact]
    public void GetHashCode_ReturnsDifferentValueForDifferentProperties()
    {
        var obj1 = new Clipping("text1", 1, 2, 3, new DateTime(2022, 1, 1));
        var obj2 = new Clipping("text2", 1, 2, 3, new DateTime(2022, 1, 1));
        
        var hashCode1 = obj1.GetHashCode();
        var hashCode2 = obj2.GetHashCode();
        
        Assert.NotEqual(hashCode1, hashCode2);
    }
    
    [Fact]
    public void ToString_ReturnsExpectedStructure()
    {
        var testSubject = new Clipping("text1", 1, 2, 3, new DateTime(2022, 1, 1));
        
        Assert.Equal("text1 (at 3 from 1 to 2 highlighted at 01.01.2022 00:00:00", testSubject.ToString());
    }
    
     [Fact]
        public void Equals_ReturnsTrueForSameObject()
        {
            var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));

            Assert.True(clipping.Equals(clipping));
        }

        [Fact]
        public void Equals_ReturnsFalseForNull()
        {
            var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            
            Assert.False(clipping.Equals(null));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentType()
        {
            var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            var otherObject = new object();

            Assert.False(clipping.Equals(otherObject));
        }

        [Fact]
        public void Equals_ReturnsTrueForEqualObjects()
        {
            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            var clipping2 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));

            Assert.True(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentText()
        {
            var clipping1 = new Clipping("text1", 1, 2, 3, new DateTime(2022, 1, 1));
            var clipping2 = new Clipping("text2", 1, 2, 3, new DateTime(2022, 1, 1));
            
            Assert.False(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentStartPosition()
        {
            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            var clipping2 = new Clipping("text", 10, 2, 3, new DateTime(2022, 1, 1));
            
            Assert.False(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentFinishPosition()
        {
            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            var clipping2 = new Clipping("text", 1, 20, 3, new DateTime(2022, 1, 1));

            Assert.False(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentPage()
        {
            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            var clipping2 = new Clipping("text", 1, 2, 30, new DateTime(2022, 1, 1));

            Assert.False(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentHighlightDate()
        {
            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            var clipping2 = new Clipping("text", 1, 2, 3, new DateTime(2023, 1, 1));

            Assert.False(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentBook()
        {
            var book1 = new Book("author1", "title1");
            var book2 = new Book("author2", "title2");
            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1)) { Book = book1 };
            var clipping2 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1)) { Book = book2 };
            
            Assert.False(clipping1.Equals(clipping2));
        }

        [Fact]
        public void Equals_ReturnsTrueForEqualBooks()
        {
            var book = new Book("author", "title");

            var clipping1 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1)) { Book = book };
            var clipping2 = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1)) { Book = book };

            Assert.True(clipping1.Equals(clipping2));
        }

}