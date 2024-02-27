using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Notion.Utils;
using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion;

public class NotionClient(string databaseId, INotionClient notionClient, IPagesUpdateParametersBuilder builder)
    : IExportClient
{

    public Task<Database> GetDatabase()
    {
        return notionClient.Databases.RetrieveAsync(databaseId);
    }

    public Task<PaginatedList<Page>> Query(Book book)
    {
        return notionClient.Databases.QueryAsync(
            databaseId,
            new DatabasesQueryParameters
            {
                Filter = new TitleFilter("Title", equal: book.Title)
            }
        );
    }

    public async Task ExportAsync(List<Book> books)
    {
        foreach (var book in books)
        {
            var pages = await this.Query(book);
            Console.WriteLine($"Found {pages.Results.Count}");

            if (pages.Results.Count == 0)
            {
                await CreateBookAsync(book);
                continue;
            }
            
            await UpdateBookAsync(book, pages.Results[0]);
        }

        Console.WriteLine($"Export finished!");
    }

    private async Task CreateBookAsync(Book book)
    {
        var page = await notionClient.Pages.CreateAsync(this.GetCreateBuilder(book));
        if (page?.Id == null)
        {
            Console.WriteLine($"Couldn't create page for book {book.Title} by {book.Author}");
        }

        Console.WriteLine($"Created page for book {book.Title} by {book.Author}");
    }

    private PagesCreateParameters GetCreateBuilder(Book book)
    {
        book.LastSynchronized = DateTime.Now;

        return PagesCreateParametersBuilder.Create(
                new DatabaseParentInput
                {
                    DatabaseId = databaseId
                }
            )
            .AddProperty("Title", new TitlePropertyValue
            {
                Title = new List<RichTextBase>()
                {
                    new RichTextText()
                    {
                        Text = new Text
                        {
                            Content = book.Title
                        }
                    }
                }
            })
            .AddProperty("Author", new RichTextPropertyValue
            {
                RichText = new List<RichTextBase>()
                {
                    new RichTextText()
                    {
                        Text = new Text
                        {
                            Content = book.Author
                        }
                    }
                }
            })
            .AddProperty("Highlights", new NumberPropertyValue
            {
                Number = book.Clippings.Count
            })
            .AddProperty("Last Synced", new DatePropertyValue
            {
                Date = new Date
                {
                    Start = book.LastSynchronized,
                }
            })
            .SetIcon(
                new EmojiObject
                {
                    Emoji = book.Emoji
                }
            )
            .SetCover(
                new ExternalFile
                {
                    External = new ExternalFile.Info
                    {
                        Url = book.Thumbnail
                    }
                }
            )
            .AddPageContent(this.CreateClippingsTable(book))
            .Build();
    }

    private TableBlock CreateClippingsTable(Book book)
    {
        var tableRows = new List<TableRowBlock>
        {
            new()
            {
                TableRow = new TableRowBlock.Info()
                {
                    Cells = new[]
                    {
                        new List<RichTextText>()
                        {
                            new()
                            {
                                Text = new Text()
                                {
                                    Content = "Clipping"
                                }
                            }
                        },
                        new List<RichTextText>()
                        {
                            new()
                            {
                                Text = new Text()
                                {
                                    Content = "Page"
                                }
                            }
                        },
                        new List<RichTextText>()
                        {
                            new()
                            {
                                Text = new Text()
                                {
                                    Content = "Start Position"
                                }
                            }
                        },
                        new List<RichTextText>()
                        {
                            new()
                            {
                                Text = new Text()
                                {
                                    Content = "Finish Position"
                                }
                            }
                        }
                    }
                }
            }
        };

        tableRows.AddRange(book.Clippings.Select(clipping => new TableRowBlock()
        {
            TableRow = new TableRowBlock.Info()
            {
                Cells = new[]
                {
                    new[] { new RichTextText() { Text = new Text() { Content = clipping.Text } } },
                    new[] { new RichTextText() { Text = new Text() { Content = clipping.Page.ToString() } } },
                    new[] { new RichTextText() { Text = new Text() { Content = clipping.StartPosition.ToString() } } },
                    new[] { new RichTextText() { Text = new Text() { Content = clipping.FinishPosition.ToString() } } }
                }
            }
        }));

        return new TableBlock()
        {
            Table = new TableBlock.Info()
            {
                TableWidth = 4,
                HasColumnHeader = true,
                HasRowHeader = false,
                Children = tableRows
            }
        };
    }

    private async Task UpdateBookAsync(Book book, Page page)
    {
        Console.WriteLine($"Book has already been synced. Therefore it's going to be updated.");

        book.LastSynchronized = DateTime.Now;

        var result = await notionClient.Pages.UpdateAsync(page.Id, GetUpdateParameters(book, page));
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
                Children = new[] { this.CreateClippingsTable(book) }
            }
        );
    }

    private PagesUpdateParameters GetUpdateParameters(Book book, Page page)
    {
        page.Properties.Remove("LastEdited");
        foreach (var property in page.Properties)
        {
            builder.WithProperty(property.Key, property.Value);
        }

        builder.WithProperty("Title", new TitlePropertyValue
            {
                Title =
                [
                    new RichTextText()
                    {
                        Text = new Text
                        {
                            Content = book.Title
                        }
                    }
                ]
            })
            .WithProperty("Author", new RichTextPropertyValue
            {
                RichText =
                [
                    new RichTextText()
                    {
                        Text = new Text
                        {
                            Content = book.Author
                        }
                    }
                ]
            })
            .WithProperty("Highlights", new NumberPropertyValue
            {
                Number = book.Clippings.Count
            })
            .WithProperty("Last Synced", new DatePropertyValue
            {
                Date = new Date
                {
                    Start = book.LastSynchronized,
                }
            });

        builder.WithCover(page.Cover);
        builder.WithIcon(new EmojiObject
        {
            Emoji = book.Emoji
        });

        return builder.Build();
    }
}