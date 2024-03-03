using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Notion.Utils;
using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion;

public class NotionClient(string databaseId, INotionClient notionClient, IPageBuilder builder)
    : IExportClient
{
    public Task<Database> GetDatabaseAsync()
    {
        return notionClient.Databases.RetrieveAsync(databaseId);
    }

    public Task<PaginatedList<Page>> QueryAsync(Book book)
    {
        return notionClient.Databases.QueryAsync(
            databaseId,
            new DatabasesQueryParameters
            {
                Filter = new TitleFilter("Title", equal: book.Title)
            }
        );
    }

    public async Task CreateAsync(Book book)
    {
        book.LastSynchronized = new DateTimeOffset(DateTime.Now);
        var page = await notionClient.Pages.CreateAsync(builder.Create(book));
        if (page?.Id == null)
        {
            Console.WriteLine($"Couldn't create page for book {book.Title} by {book.Author}");
        }

        Console.WriteLine($"Created page for book {book.Title} by {book.Author}");
    }

    public async Task UpdateAsync(Book book, Page page)
    {
        book.LastSynchronized = new DateTimeOffset(DateTime.Now);
        Console.WriteLine($"Book has already been synced. Therefore it's going to be updated.");

        book.LastSynchronized = new DateTimeOffset(DateTime.Now);

        var result = await notionClient.Pages.UpdateAsync(page.Id, builder.Update(page, book));
        if (result?.Id == null)
        {
            throw new Exception($"Properties of page ${page.Id} couldn't be updated.");
        }

        var children = await notionClient.Blocks.RetrieveChildrenAsync(page.Id);
        foreach (var child in children.Results)
        {
            await notionClient.Blocks.DeleteAsync(child.Id);
        }

        await notionClient.Blocks.AppendChildrenAsync(
            page.Id,
            new BlocksAppendChildrenParameters()
            {
                Children = new[] { builder.CreateClippingsTable(book) }
            }
        );
    }
}