using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Model;

namespace ExportKindleClippingsToNotion.Parser;

public interface IBooksParser
{
    public Task<List<Book>> Parse(IEnumerable<string> clippings);
}

public class BooksParser : IBooksParser
{
    private readonly IBookMetadataFetcher _metadataFetcher;
    private readonly IClippingsParser _clippingsParser;

    public BooksParser(IBookMetadataFetcher metadataFetcher, IClippingsParser clippingsParser)
    {
        _metadataFetcher = metadataFetcher;
        _clippingsParser = clippingsParser;
    }

    public Task<List<Book>> Parse(IEnumerable<string> clippings)
    {
        return ParseBooks(clippings);
    }

    private async Task<List<Book>> ParseBooks(IEnumerable<string> clippings)
    {
        var books = new List<Book>();
        var parsedClippings = new List<Clipping>();

        foreach (var clipping in clippings)
        {
            var dto = await _clippingsParser.Parse(clipping);
            if (dto?.Clipping == null || dto?.Author == null || dto?.Title == null)
            {
                continue;
            }

            var book = books.Find(x => x.Author == dto.Author && x.Title == dto.Title);
            if (book is null)
            {
                book = new Book(dto.Author, dto.Title);
                await AddThumbnail(book);
                books.Add(book);
            }

            dto.Clipping.Book = book;
            book.AddClipping(dto.Clipping);
            parsedClippings.Add(dto.Clipping);
        }

        Console.WriteLine($"Parsed {books.Count} books");
        Console.WriteLine($"Parsed {parsedClippings.Count} clippings");

        return books;
    }

    private async Task AddThumbnail(Book book)
    {
        book.Thumbnail = await _metadataFetcher.SearchThumbnail(book);
        if (book.Thumbnail == null || book.Thumbnail.Trim().Length == 0)
        {
            //TODO: Use fallback image
            Console.WriteLine($"use fallback image for {book.Title}");
            return;
        }

        Console.WriteLine($"Found thumbnail for {book.Title}");
    }
}