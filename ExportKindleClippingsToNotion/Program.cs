﻿using System.IO.Abstractions;
using ExportKindleClippingsToNotion.Export;
using ExportKindleClippingsToNotion.Import;
using ExportKindleClippingsToNotion.Import.Metadata;
using ExportKindleClippingsToNotion.Notion.Utils;
using ExportKindleClippingsToNotion.Parser;
using Notion.Client;
using BooksService = ExportKindleClippingsToNotion.Import.Metadata.BooksService;
using NotionClient = ExportKindleClippingsToNotion.Notion.NotionClient;

try
{
    var arguments = Arguments.Parse(args);
    if (!arguments.IsParseSuccessful)
    {
        Console.WriteLine(
            "Please provide a path to your clippings file, your Notion Authentication Token and your Notion Database ID. Use --help for more information.");
        return;
    }

    var options = arguments.ParsedOptions!;
    var fileSystem = new FileSystem();

    var notionClient = NotionClientFactory.Create(
        new ClientOptions
        {
            AuthToken = options.NotionAuthenticationToken
        }
    );
    var client = new NotionClient(options.NotionDatabaseId, notionClient, new PagesUpdateParametersBuilder());
    var importer = new Importer(new FileClient(fileSystem));
    var exporter = new Exporter(client);

    var metadataFetcher = new GoogleBooksClient(new BooksService());
    var clippingsParser = new ClippingsParserGerman();
    var booksParser = new BooksParser(metadataFetcher, clippingsParser);

    var exportKindleClippingsToNotion =
        new ExportKindleClippingsToNotion.ExportKindleClippingsToNotion(importer, booksParser, exporter);
    await exportKindleClippingsToNotion.ExecuteAsync(options.PathToClippings);
}
catch (Exception exception)
{
    Console.WriteLine($"An error occurred: {exception}");
}