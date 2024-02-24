using ExportKindleClippingsToNotion.Model;
using Google.Apis.Books.v1;
using Google.Apis.Services;

namespace ExportKindleClippingsToNotion.Import.Metadata;

public interface IBookMetadataFetcher
{
    public Task<string?> SearchThumbnail(Book book);
}

public class GoogleBooksClient(IBooksService service) : IBookMetadataFetcher
{
    public async Task<string?> SearchThumbnail(Book book)
    {
        var query = $"intitle:{book.Title}+inauthor:{book.Author}";
        var volumes = await service.ExecuteVolumesListRequestAsync(query);

        if (volumes.Items == null || volumes.Items.Count == 0)
        {
            return null;
        }

        return volumes.Items[0]?.VolumeInfo.ImageLinks.Thumbnail;
    }
}