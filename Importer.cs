using Notion.Client;

namespace ExportKindleClippingsToNotion;

internal interface IImportClient
{
    Task<PaginatedList<Page>> Query(Book book);
}