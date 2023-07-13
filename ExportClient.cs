//TODO: DI?
using Notion.Client;

//TODO: Return values depend on notion client
interface IExporter
{
    //TODO: Book interface
    Task<PaginatedList<Page>> query(Book book);

    void export(List<Book> books);
}

class NotionExporter : IExporter
{
    private readonly NotionClient client;

    private string authenticationToken;

    private readonly string databaseId;

    public NotionExporter(string AuthenticationToken, string databaseId)
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
                //TODO: Constant for title value
                //TODO: Filter author?
                Filter = new TitleFilter("Title", equal: book.Title)
            }
        );
    }

    public async void export(List<Book> books)
    {
        foreach (var book in books)
        {
            var pages = await this.query(book);
            Console.WriteLine($"Found {pages.Results.Count}");

            if (pages.Results.Any())
            {
                //TODO: Update the page's content with the current clippings
                Console.WriteLine($"Book has already been synced. Therefore it's going to be updated.");
                continue;
            }

            var builder = PagesCreateParametersBuilder.Create(
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

            //TODO: Call notion client & validate result
            var result = await client.Pages.CreateAsync(builder.Build());

            Console.WriteLine($"Created page for book {book.Author}");
        }
    }
}