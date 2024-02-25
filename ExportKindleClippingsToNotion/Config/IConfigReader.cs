namespace ExportKindleClippingsToNotion.Config;

public interface IConfigReader
{
    Task<Config> ExecuteAsync(string pathToConfig);
}