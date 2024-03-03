using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;

namespace ExportKindleClippingsToNotion.Import.Metadata;

public interface IBooksService
{
    Task<Volumes> ExecuteVolumesListRequestAsync(string query);
}