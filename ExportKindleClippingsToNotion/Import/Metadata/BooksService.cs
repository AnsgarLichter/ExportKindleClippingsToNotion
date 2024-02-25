using Google.Apis.Books.v1.Data;
using Google.Apis.Services;

namespace ExportKindleClippingsToNotion.Import.Metadata;

class BooksService : IBooksService
{
    private readonly Google.Apis.Books.v1.BooksService _service = new(new BaseClientService.Initializer());

    public async Task<Volumes> ExecuteVolumesListRequestAsync(string query)
    {
        return await _service.Volumes.List(query).ExecuteAsync();
    }
}