using ExportKindleClippingsToNotion.Model;
using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion.Utils;

public class PageBuilder : IPageBuilder
{
    private readonly string _databaseId;
    private readonly IPagesUpdateParametersBuilder _updateBuilder;

    public PageBuilder(string databaseId, IPagesUpdateParametersBuilder updateBuilder)
    {
        _databaseId = databaseId;
        _updateBuilder = updateBuilder;
    }

    public PagesCreateParameters Create(Book book)
    {
        return PagesCreateParametersBuilder.Create(
                new DatabaseParentInput
                {
                    DatabaseId = _databaseId
                }
            )
            .AddProperty("Title", new TitlePropertyValue
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
            .AddProperty("Author", new RichTextPropertyValue
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
            .AddProperty("Highlights", new NumberPropertyValue
            {
                Number = book.Clippings.Count
            })
            .AddProperty("Last Synchronized", new DatePropertyValue
            {
                Date = new Date
                {
                    Start = book.LastSynchronized?.LocalDateTime,
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
                        Url = book.ThumbnailUrl
                    }
                }
            )
            .AddPageContent(CreateClippingsTable(book))
            .Build();
    }

    public IBlock CreateClippingsTable(Book book)
    {
        var tableRows = new List<TableRowBlock>
        {
            new()
            {
                TableRow = new TableRowBlock.Info()
                {
                    Cells = new[]
                    {
                        [
                            new RichTextText
                            {
                                Text = new Text()
                                {
                                    Content = "Clipping"
                                }
                            }
                        ],
                        [
                            new RichTextText
                            {
                                Text = new Text()
                                {
                                    Content = "Page"
                                }
                            }
                        ],
                        [
                            new RichTextText
                            {
                                Text = new Text()
                                {
                                    Content = "Start Position"
                                }
                            }
                        ],
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

    public PagesUpdateParameters Update(Page page, Book book)
    {
        page.Properties.Remove("Last Edited");
        foreach (var property in page.Properties)
        {
            _updateBuilder.WithProperty(property.Key, property.Value);
        }

        return _updateBuilder.WithProperty("Title", new TitlePropertyValue
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
            .WithProperty("Last Synchronized", new DatePropertyValue
            {
                Date = new Date
                {
                    Start = book.LastSynchronized?.LocalDateTime,
                }
            })
            .WithCover(page.Cover)
            .WithIcon(new EmojiObject
            {
                Emoji = book.Emoji
            }).Build();
    }
}