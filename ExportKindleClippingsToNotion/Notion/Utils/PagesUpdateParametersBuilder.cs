using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion.Utils;

public class PagesUpdateParametersBuilder : IPagesUpdateParametersBuilder
{
    private Dictionary<string, PropertyValue> _properties = new();
    private FileObject? _cover = null;
    private IPageIcon? _icon = null;
    private bool _isArchived = false;

    public IPagesUpdateParametersBuilder WithProperty(string nameOrId, PropertyValue value)
    {
        _properties[nameOrId] = value;

        return this;
    }

    public IPagesUpdateParametersBuilder WithIcon(IPageIcon pageIcon)
    {
        _icon = pageIcon;

        return this;
    }

    public IPagesUpdateParametersBuilder WithCover(FileObject cover)
    {
        _cover = cover;

        return this;
    }

    public IPagesUpdateParametersBuilder WithIsArchived(bool isArchived)
    {
        _isArchived = isArchived;

        return this;
    }

    public PagesUpdateParameters Build()
    {
        return new PagesUpdateParameters
        {
            Properties = _properties,
            Icon = _icon,
            Cover = _cover,
            Archived = _isArchived
        };
    }

    public IPagesUpdateParametersBuilder Reset()
    {
        _properties = new();
        _cover = null;
        _icon = null;
        _isArchived = false;

        return this;
    }
}