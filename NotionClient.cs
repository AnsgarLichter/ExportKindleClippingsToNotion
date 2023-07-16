//TODO: DI?
using Notion.Client;

class NotionClient : IExportClient, IImportClient
{
    private readonly Notion.Client.NotionClient client;

    private string authenticationToken;

    private readonly string databaseId;

    public NotionClient(string AuthenticationToken, string databaseId)
    {
        this.authenticationToken = AuthenticationToken;
        this.databaseId = databaseId;
        this.client = NotionClientFactory.Create(
            new ClientOptions
            {
                AuthToken = authenticationToken
            }
        );
    }

    private Task<Database> getDatabase()
    {
        return client.Databases.RetrieveAsync(this.databaseId);
    }

    public Task<PaginatedList<Page>> query(Book book)
    {
        return client.Databases.QueryAsync(
                   this.databaseId,
                   new DatabasesQueryParameters
                   {
                       Filter = new TitleFilter("Title", equal: book.Title)
                   }
        );
    }

    public async void export(List<Book> books)
    {
        foreach (var book in books)
        {
            var task = this.query(book);
            task.Wait();
            var pages = task.Result;
            Console.WriteLine($"Found {pages.Results.Count}");

            if (pages.Results.Any())
            {
                this.updateBook(book);
                continue;
            }

            this.createBook(book);
        }

        Console.WriteLine($"Export finished!");
    }

    private async void createBook(Book book)
    {
        PagesCreateParametersBuilder builder = this.getBuilder(book);
        this.addClippings(book, builder);

        //TODO: Call notion client & validate result
        var task = client.Pages.CreateAsync(builder.Build());
        task.Wait();
        var page = task.Result;

        if (page == null || page.Id == null)
        {
            Console.WriteLine($"Couldn't create page for book {book.Title} by {book.Author}");    
        }
        Console.WriteLine($"Created page for book {book.Title} by {book.Author}");
    }

    private PagesCreateParametersBuilder getBuilder(Book book)
    {
        book.LastSynchronized = DateTime.Now;

        return PagesCreateParametersBuilder.Create(
                new DatabaseParentInput
                {
                    DatabaseId = this.databaseId
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
            );
    }

    private void addClippings(Book book, PagesCreateParametersBuilder builder)
    {
        foreach (var clipping in book.Clippings)
        {
            //TODO: Improve UI of the found clippings - use a table?
            builder.AddPageContent(
                new BulletedListItemBlock()
                {
                    BulletedListItem = new BulletedListItemBlock.Info
                    {
                        RichText = new List<RichTextBase>
                        {
                            new RichTextText
                            {
                                Text = new Text
                                {
                                    Content = $"{clipping.Text} (at Page {clipping.Page})"
                                }
                            }
                        }
                    }
                }
            );
        }
    }

    private async void updateBook(Book book)
    {
        Console.WriteLine($"Book has already been synced. Therefore it's going to be updated.");

        //TODO: Update Book's properties and clippings in Notion
        return;
    }
}