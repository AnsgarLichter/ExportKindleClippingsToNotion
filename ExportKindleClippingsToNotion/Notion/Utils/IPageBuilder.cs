using ExportKindleClippingsToNotion.Model;
using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion.Utils;

public interface IPageBuilder
{
    PagesCreateParameters Create(Book book);
    PagesUpdateParameters Update(Page page, Book book);
    IBlock CreateClippingsTable(Book book);
}