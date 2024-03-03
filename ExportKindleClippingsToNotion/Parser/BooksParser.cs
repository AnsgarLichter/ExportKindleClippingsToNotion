using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Model.Dto;

namespace ExportKindleClippingsToNotion.Parser;

public class BooksParser(IBookMetadataFetcher metadataFetcher, IClippingsParser clippingsParser)
    : IBooksParser
{
    private const string FallbackThumbnailUrl =
        "https://bookstoreromanceday.org/wp-content/uploads/2020/08/book-cover-placeholder.png";

    public async Task<List<Book>> ParseAsync(IEnumerable<string> clippings)
    {
        return await ParseBooksAsync(clippings);
    }

    private async Task<List<Book>> ParseBooksAsync(IEnumerable<string> clippings)
    {
        var books = new List<Book>();
        var parsedClippings = new List<Clipping>();

        foreach (var clipping in clippings)
        {
            var dto = clippingsParser.Parse(clipping);
            if (dto?.Text == null || dto?.Text.Trim().Length == 0 || dto?.Author == null || dto?.Title == null)
            {
                continue;
            }

            var book = books.Find(x => x.Author == dto.Author && x.Title == dto.Title);
            if (book is null)
            {
                book = new Book(dto.Author, dto.Title)
                {
                    ThumbnailUrl = await AddThumbnailAsync(new BookDto()
                    {
                        Author = dto.Author,
                        Title = dto.Title
                    })
                };
                books.Add(book);
            }

            var parsedClipping = new Clipping(
                dto.Text,
                dto.StartPosition,
                dto.FinishPosition,
                dto.Page,
                dto.HighlightDate,
                book
            );
            book.AddClipping(parsedClipping);
            parsedClippings.Add(parsedClipping);
        }

        Console.WriteLine($"Parsed {books.Count} books");
        Console.WriteLine($"Parsed {parsedClippings.Count} clippings");

        return books;
    }

    private async Task<string> AddThumbnailAsync(BookDto book)
    {
        var thumbnail = await metadataFetcher.GetThumbnailUrlAsync(book);
        if (thumbnail == null || thumbnail.Trim().Length == 0)
        {
            Console.WriteLine($"use fallback image for {book.Title}");
            return FallbackThumbnailUrl;
        }

        Console.WriteLine($"Found thumbnail for {book.Title}");
        return thumbnail;
    }
}