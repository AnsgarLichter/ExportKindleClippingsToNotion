using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Model;
using FakeItEasy;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using JetBrains.Annotations;

namespace UnitTests.Import.Metadata;

[TestSubject(typeof(GoogleBooksClient))]
public class GoogleBooksClientTest
{
    private readonly IBooksService _booksServiceMock;
    private readonly GoogleBooksClient _testSubject;

    public GoogleBooksClientTest()
    {
        _booksServiceMock = A.Fake<IBooksService>();
        _testSubject = new GoogleBooksClient(_booksServiceMock);
    }
    
    [Fact]
    public async Task SearchThumbnail_ReturnsThumbnail()
    {
        var book = new Book("author", "title");
        const string thumbnailUrl = "https://example.com/thumbnail.jpg";
        var volumes = new Volumes()
        {
            Items = new[]
            {
                new Volume()
                {
                    VolumeInfo = new Volume.VolumeInfoData()
                    {
                        ImageLinks = new Volume.VolumeInfoData.ImageLinksData()
                        {
                            Thumbnail = thumbnailUrl
                        }
                    }
                }
            }
        };
        A.CallTo(() => _booksServiceMock.ExecuteVolumesListRequestAsync("intitle:title+inauthor:author"))
            .Returns(volumes);
        
        var result = await _testSubject.SearchThumbnailAsync(book);
        
        Assert.Equal(thumbnailUrl, result);
    }
    
    [Fact]
    public async Task SearchThumbnail_ReturnsNullWhenZeroItemsFound()
    {
        var book = new Book("author", "title");
        const string thumbnailUrl = "https://example.com/thumbnail.jpg";
        var volumes = new Volumes()
        {
            Items = []
        };
        
        A.CallTo(() => _booksServiceMock.ExecuteVolumesListRequestAsync("intitle:title+inauthor:author"))
            .Returns(volumes);
        
        var result = await _testSubject.SearchThumbnailAsync(book);
        
        Assert.Null(result);
    }
}