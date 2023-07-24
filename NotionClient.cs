//TODO: DI?

using Notion.Client;

namespace ExportKindleClippingsToNotion;

class NotionClient : IExportClient, IImportClient
{
    private readonly Notion.Client.NotionClient _client;

    private readonly string _databaseId;

    public NotionClient(string authenticationToken, string databaseId)
    {
        this._databaseId = databaseId;
        this._client = NotionClientFactory.Create(
            new ClientOptions
            {
                AuthToken = authenticationToken
            }
        );
    }

    private Task<Database> GetDatabase()
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

    public async void Export(List<Book> books)
    {
        foreach (var book in books)
        {
            //TODO: Shouldn't this be checked while importing?
            var pages = await this.Query(book);
            Console.WriteLine($"Found {pages.Results.Count}");

            if (pages.Results.Any())
            {
                this.UpdateBook(book, pages.Results[0]);
                continue;
            }

            this.CreateBook(book);
        }

        Console.WriteLine($"Export finished!");
    }

    private async void CreateBook(Book book)
    {
        var builder = this.GetCreateBuilder(book);
        this.AddClippings(book, builder);

        //TODO: Call notion client & validate result
        var page = await _client.Pages.CreateAsync(builder.Build());
        if (page?.Id == null)
        {
            Console.WriteLine($"Couldn't create page for book {book.Title} by {book.Author}");
        }

        Console.WriteLine($"Created page for book {book.Title} by {book.Author}");
    }

    private PagesCreateParametersBuilder GetCreateBuilder(Book book)
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
            );
    }

    private void AddClippings(Book book, PagesCreateParametersBuilder builder)
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

    private async void UpdateBook(Book book, Page page)
    {
        Console.WriteLine($"Book has already been synced. Therefore it's going to be updated.");

        book.LastSynchronized = DateTime.Now;

        //TODO: Update Properties
        var builder = getUpdateBuilder(book, page);

        var result = await _client.Pages.UpdateAsync(page.Id, builder.Build());
        if (result?.Id == null)
        {
            throw new Exception($"Properties of page ${page.Id} couldn't be updated.");
        }

        //TODO: Update Content of Page
        return;
    }

    private static PagesUpdateParametersBuilder getUpdateBuilder(Book book, Page page)
    {
        var builder = PagesUpdateParametersBuilder.Create(page)
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
            );
        return builder;
    }
}

public class PagesUpdateParametersBuilder
{
    private Dictionary<string, PropertyValue> _properties = new();
    private FileObject? _cover;
    private IPageIcon? _icon;
    private bool _isArchived;

    private PagesUpdateParametersBuilder()
    {
    }

    public static PagesUpdateParametersBuilder Create(Page page)
    {
        var builder = new PagesUpdateParametersBuilder()
        {
            _properties = (Dictionary<string, PropertyValue>)page.Properties,
            _cover = page.Cover,
            _icon = page.Icon,
            _isArchived = page.IsArchived
        };

        builder._properties.Remove("Last Edited");

        return builder;
    }

    public PagesUpdateParametersBuilder AddOrUpdateProperty(string nameOrId, PropertyValue value)
    {
        this._properties[nameOrId] = value;

        return this;
    }

    public PagesUpdateParametersBuilder SetIcon(IPageIcon pageIcon)
    {
        this._icon = pageIcon;

        return this;
    }

    public PagesUpdateParametersBuilder SetCover(FileObject cover)
    {
        this._cover = cover;

        return this;
    }

    public PagesUpdateParameters Build()
    {
        return new PagesUpdateParameters
        {
            Properties = this._properties,
            Icon = this._icon,
            Cover = this._cover,
            Archived = this._isArchived
        };
    }
}