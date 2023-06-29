namespace Ziggio.Csv.Configuration;

/// <summary>
/// A class used to bind appsettings
/// </summary>
public class CsvConfigSection {
  public CsvReaderConfigSection CsvReader { get; set; }

  public class CsvReaderConfigSection {
    public bool ContainsHeaderRow { get; set; }
    public string Delimiter { get; set; }
    public bool IgnoreRowsWithErrors { get; set; }
    public bool IsCommaQuoteDelimited { get; set; }
    public string NewLine { get; set; }
    public string QuoteCharacter { get; set; }
    public bool RemoveFieldQuotes { get; set; }
  }
}
