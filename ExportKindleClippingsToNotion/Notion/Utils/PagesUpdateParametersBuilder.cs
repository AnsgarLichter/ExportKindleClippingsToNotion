using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion.Utils;

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