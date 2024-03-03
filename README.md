<div align="center" width="100%">
    <img src="https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/assets/43957906/0539f156-7d9c-40f7-bc42-23f33c971458" width="150" />
</div>

<div align="center" width="100%">
    <h2>ExportKindleClippingsToNotion</h2>
    <p>Simple and fast synchronization of your kindle clippings. Just plugin your Kindle to your PC and run the CLI tool.</p>
    <a target="_blank" href="https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/stargazers"><img src="https://img.shields.io/github/stars/AnsgarLichter/ExportKindleClippingsToNotion" /></a>
    <a target="_blank" href="https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/releases"><img src="https://img.shields.io/github/v/release/AnsgarLichter/ExportKindleClippingsToNotion?display_name=tag" /></a>
    <a target="_blank" href="https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/main"><img src="https://img.shields.io/github/last-commit/AnsgarLichter/ExportKindleClippingsToNotion" /></a>
    <a target="_blank" href="https://www.nuget.org/packages/ExportKindleClippingsToNotion"><img src="https://img.shields.io/nuget/v/ExportKindleClippingsToNotion" /></a>
</div>


## ✨ Features
- [x] Import Clippings from Kindle's file
- [X] Book Covers from Google Books API
- [X] Export Clippings to Notion
- [X] Update existing books in Notion
- [x] German and English language support. Please open an [issue](https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/issues) if you want to support more languages.

## 🤔 Why?

I just wanted to have a tool to sync my kindle clippings to Notion. As other tools don't support my mother langauge German I created a tool that does it and is extendable to add the support to other languages easily.
If you want to request the support for your language, open an issue and add an example clippings file for your requested language.

## 🔧 Prerequisites
- Kindle
- Notion account
- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or higher

## :hammer: Setup and Installation
1. Install the tool:
   - Download the latest release from the [releases page](https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/releases)
   - Using Nuget (see [ExportKindleClippingsToNotion@Nuget](https://www.nuget.org/packages/ExportKindleClippingsToNotion/1.0.0):
     ```bash
       dotnet tool install --global ExportKindleClippingsToNotion --version 1.0.0
     ```
2. Duplicate the [database template](https://skillful-lemming-b3c.notion.site/8953e78fa1264d5c9913e31883240fa0?v=914febedb5c04978ad07372bb57d1248&pvs=74) into your own workspace.
3. Create a `New integration` at [Notion - My Integrations](https://www.notion.so/my-integrations). As soon as your created your integration, press `Secrets`, `Show` and `Copy` your token. Save it somewhere to use it later.
4. Share your database with your integration:
    ![image](https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/assets/43957906/bfd9d093-017a-4da2-9476-c17d03567191)
    - Open your database
    - Press the `...` at the top right corner to open the context menu
    - Select `Connect to`
    - Enter the name of your integration, select it and confirm the connection
5. Retrieve your database's ID (see also [Retrieve a database](https://developers.notion.com/reference/retrieve-a-database)):
    - Open your database in your browser.
    - Copy to URL and extract the ID:
      ```txt
      https://www.notion.so/databaseId?v=231321...
      ```

## 🚀 Usage
1. Plugin your Kindle to your PC
2. Copy the absolute path to your Kindle's clippings file
3. Provide the arguments as shown in the help. `path`, `authenticationToken` and `databaseId` are required.
   ```bash
   ExportKindleClippingsToNotion 1.0.0+dbe3b7b917b3bd522e6577e65359e273e3e95601
   Copyright (C) 2024 ExportKindleClippingsToNotion
    
      -p, --path                   Required. Path to the clippings file
    
      -a, --authenticationToken    Required. Your Notion Authentication Token to access the Notion API.
    
      -d, --databaseId             Required. Your Notion Database ID to export the clippings to.
    
      --help                       Display this help screen.
    
      --version                    Display version information.
    
    Please provide a path to your clippings file, your Notion Authentication Token and your Notion Database ID. Use --help for more information.
   ```

   A call may look like this:
   ```
   ExportKindleClippingsToNotion -p ~/Users/Test/Documents/Clippings.txt -a secret_kashjdsajhkdsdjshjsdahdkjdshajkhs -d 1253e78fs1214d5c9913e31883240fa0
   ```

## :sparkler: Outlook
Later releases of the app can support more features. Current ideas are:
- [ ] Support for more languages
- [ ] Additional support the amazon homepage as a source for the clippings if there are all information from the clippings file available

If you have other ideas or example clippings files for other languages please feel free to open an [issue](https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/issues)!
If you have questions, please open a [discussion](https://github.com/AnsgarLichter/ExportKindleClippingsToNotion/discussions).
