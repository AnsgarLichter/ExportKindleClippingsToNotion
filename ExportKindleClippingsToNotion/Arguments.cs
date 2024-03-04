using CommandLine;
using ExportKindleClippingsToNotion;

public class Arguments
{
    private readonly ParserResult<Options> _parserResult;

    private Arguments(ParserResult<Options> parserResult) => _parserResult = parserResult;

    public Options? ParsedOptions => (_parserResult as Parsed<Options?>)?.Value;

    public bool IsParseSuccessful => _parserResult.Tag == ParserResultType.Parsed;

    public static Arguments Parse(IEnumerable<string> arguments) =>
        new(Parser.Default.ParseArguments<Options>(arguments));
}