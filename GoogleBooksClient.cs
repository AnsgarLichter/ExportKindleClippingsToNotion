using Google.Apis.Books.v1;
using Google.Apis.Services;

namespace ExportKindleClippingsToNotion;

public interface IBookMetadataFetcher
{
    public Task<string?> SearchThumbnail(Book book);
}

public class GoogleBooksClient: IBookMetadataFetcher
{
    private readonly BooksService _service = new(new BaseClientService.Initializer());

    public async Task<string?> SearchThumbnail(Book book)
    {
        var query = $"intitle:{book.Title}+inauthor:{book.Author}";
        var volumes = await this._service.Volumes.List(query).ExecuteAsync();

        if (volumes.Items == null || volumes.Items.Count == 0)
        {
            return null;
        }

        return volumes.Items[0]?.VolumeInfo.ImageLinks.Thumbnail;
    }
}