using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Model;
using ExportKindleClippingsToNotion.Notion.Utils;
using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion;

class NotionClient : IExportClient
{
    private readonly global::Notion.Client.NotionClient _client;

    private readonly string _databaseId;

    public NotionClient(string authenticationToken, string databaseId)
    {
        this._databaseId = databaseId;
        //TODO: DI?
        this._client = NotionClientFactory.Create(
            new ClientOptions
            {
                AuthToken = authenticationToken
            }
        );
    }

    public Task<Database> GetDatabase()
    {
        return _client.Databases.RetrieveAsync(this._databaseId);
    }

    public Task<PaginatedList<Page>> Query(Book book)
    {
        return _client.Databases.QueryAsync(
            this._databaseId,
            new DatabasesQueryParameters
            {
                Filter = new TitleFilter("Title", equal: book.Title)
            }
        );
    }

    public async Task Export(List<Book> books)
    {
        foreach (var book in books)
        {
            var pages = await this.Query(book);
            Console.WriteLine($"Found {pages.Results.Count}");
            
            if (pages.Results.Any())
            {
                //TODO: Only update if clippings count differs. The current synced clipping count is available in the properties of the page
                await this.UpdateBook(book, pages.Results[0]);
                continue;
            }

            await this.CreateBook(book);
        }

        Console.WriteLine($"Export finished!");
    }

    private async Task CreateBook(Book book)
    {
        var page = await _client.Pages.CreateAsync(this.GetCreateBuilder(book));
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
                    DatabaseId = this._databaseId
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

    private async Task UpdateBook(Book book, Page page)
    {
        Console.WriteLine($"Book has already been synced. Therefore it's going to be updated.");

        book.LastSynchronized = DateTime.Now;
        
        var result = await _client.Pages.UpdateAsync(page.Id, GetUpdateBuilder(book, page));
        if (result?.Id == null)
        {
            throw new Exception($"Properties of page ${page.Id} couldn't be updated.");
        }
        
        //TODO: Only update if clippings count differs
        var children = await this._client.Blocks.RetrieveChildrenAsync(page.Id);
        foreach (var child in children.Results)
        {
            await this._client.Blocks.DeleteAsync(child.Id);
        }

        await this._client.Blocks.AppendChildrenAsync(
            page.Id,
            new BlocksAppendChildrenParameters()
            {
                Children = new[] { this.CreateClippingsTable(book) }
            }
        );
    }

    private static PagesUpdateParameters GetUpdateBuilder(Book book, Page page)
    {
        return PagesUpdateParametersBuilder.Create(page)
            .AddOrUpdateProperty("Title", new TitlePropertyValue
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
            .AddOrUpdateProperty("Author", new RichTextPropertyValue
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
            .AddOrUpdateProperty("Highlights", new NumberPropertyValue
            {
                Number = book.Clippings.Count
            })
            .AddOrUpdateProperty("Last Synced", new DatePropertyValue
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
            .Build();
    }
}