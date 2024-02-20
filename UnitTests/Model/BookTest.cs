using ExportKindleClippingsToNotion.Model;
using JetBrains.Annotations;

namespace UnitTests.Model;

[TestSubject(typeof(Book))]
public class BookTest
{

    [Fact]
        public void Equals_ReturnsTrueForSameObject()
        {
            var book = new Book("Author", "Title");
            
            Assert.True(book.Equals(book));
        }

        [Fact]
        public void Equals_ReturnsFalseForNull()
        {
            var book = new Book("Author", "Title");

            Assert.False(book.Equals(null));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentType()
        {
            var book = new Book("Author", "Title");
            var otherObject = new object();

            Assert.False(book.Equals(otherObject));
        }

        [Fact]
        public void Equals_ReturnsTrueForEqualObjects()
        {
            // Arrange
            var book1 = new Book("Author", "Title");
            var book2 = new Book("Author", "Title");
            
            Assert.True(book1.Equals(book2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentAuthor()
        {
            var book1 = new Book("Author1", "Title");
            var book2 = new Book("Author2", "Title");

            Assert.False(book1.Equals(book2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentTitle()
        {
            var book1 = new Book("Author", "Title1");
            var book2 = new Book("Author", "Title2");

            Assert.False(book1.Equals(book2));
        }

        [Fact]
        public void GetHashCode_ReturnsSameValueForEqualObjects()
        {
            var book1 = new Book("Author", "Title");
            var book2 = new Book("Author", "Title");
            
            Assert.Equal(book1.GetHashCode(), book2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ReturnsDifferentValueForDifferentObjects()
        {
            var book1 = new Book("Author1", "Title1");
            var book2 = new Book("Author2", "Title2");

            Assert.NotEqual(book1.GetHashCode(), book2.GetHashCode());
        }

        [Fact]
        public void AddClipping_AddsClippingToList()
        {
            var book = new Book("Author", "Title");
            var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            
            book.AddClipping(clipping);
            
            Assert.Contains(clipping, book.Clippings);
        }

        [Fact]
        public void AddClipping_DoesNotAddDuplicateClipping()
        {
            var book = new Book("Author", "Title");
            var clipping = new Clipping("text", 1, 2, 3, new DateTime(2022, 1, 1));
            
            book.AddClipping(clipping);
            book.AddClipping(clipping);
            
            Assert.Single(book.Clippings);
        }
        
        [Fact]
        public void Emoji_ReturnsBookEmoji()
        {
            var book = new Book("Author", "Title");
            
            Assert.Equal("📖", book.Emoji);
        }
        
        [Fact]
        public void LastSynchronized_GetterAndSetter_WorkAsExpected()
        {
            var book = new Book("Author", "Title");
            var expectedDateTime = new DateTime(2023, 1, 1);
            
            book.LastSynchronized = expectedDateTime;
            var retrievedDateTime = book.LastSynchronized;
            
            Assert.Equal(expectedDateTime, retrievedDateTime);
        }
        
        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var book = new Book("Author", "Title");
            
            Assert.Equal("Title (Author)", book.ToString());
        }
}