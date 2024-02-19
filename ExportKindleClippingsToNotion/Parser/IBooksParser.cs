using ExportKindleClippingsToNotion.Model;

namespace ExportKindleClippingsToNotion.Parser;

public interface IBooksParser
{
    public Task<List<Book>> ParseAsync(IEnumerable<string> clippings);
}