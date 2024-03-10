using System.Reflection;
using EnumStringValues;

namespace ExportKindleClippingsToNotion.Parser;

/// <summary>
/// Contains the supported languages for the clippings parser. The values must match Iso639_3.
/// </summary>
public enum SupportedLanguages
{
   [StringValue("ENG")]
   English,
   [StringValue("DEU")]
   German,
   [StringValue("RUS")]
   Russian
}