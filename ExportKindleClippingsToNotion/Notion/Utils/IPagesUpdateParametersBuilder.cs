using Notion.Client;

namespace ExportKindleClippingsToNotion.Notion.Utils;

public interface IPagesUpdateParametersBuilder
{
    
    public PagesUpdateParameters Build();

    public IPagesUpdateParametersBuilder WithProperty(string nameOrId, PropertyValue value);

    public IPagesUpdateParametersBuilder WithIcon(IPageIcon pageIcon);

    public IPagesUpdateParametersBuilder WithCover(FileObject cover);
    
    public IPagesUpdateParametersBuilder WithIsArchived(bool isArchived);
    
    public IPagesUpdateParametersBuilder Reset();
}