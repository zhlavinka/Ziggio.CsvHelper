namespace Ziggio.Csv.Configuration;

/// <summary>
/// A class used to bind appsettings
/// </summary>
public class CsvConfigSection {
  public CsvParserConfigSection CsvParser { get; set; }

  public class CsvParserConfigSection {
    public bool ContainsHeaderRow { get; set; }
    public string Delimiter { get; set; }
    public bool IgnoreBadData { get; set; }
    public bool IsCommaQuoteDelimited { get; set; }
    public string NewLine { get; set; }
    public string QuoteCharacter { get; set; }
    public bool RemoveFieldQuotes { get; set; }
    public bool ThrowErrorOnBadData { get; set; }
  }
}
