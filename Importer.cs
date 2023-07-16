using Notion.Client;

interface IImportClient
{
    Task<PaginatedList<Page>> query(Book book);

}