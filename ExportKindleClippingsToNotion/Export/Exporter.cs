using ExportKindleClippingsToNotion.Model;
using Notion.Client;

namespace ExportKindleClippingsToNotion.Export;

public interface IExporter
{
    Task ExportAsync(List<Book> books);
}

public interface IExportClient
{
    Task<PaginatedList<Page>> QueryAsync(Book book);

    Task<Database> GetDatabaseAsync();

    Task CreateAsync(Book book);

    Task UpdateAsync(Book book, Page page);
}

public class Exporter(IExportClient client) : IExporter
{
    public async Task ExportAsync(List<Book> books)
    {
        Console.WriteLine($"Starting export.");
        foreach (var book in books)
        {
            Console.WriteLine($"Exporting book {book.Title} by {book.Author}");
            var pages = await client.QueryAsync(book);

            if (pages.Results.Count == 0)
            {
                await client.CreateAsync(book);
                continue;
            }

            await client.UpdateAsync(book, pages.Results[0]);
        }

        Console.WriteLine($"Export finished!");
    }
}