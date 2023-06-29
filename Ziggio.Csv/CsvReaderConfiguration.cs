using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Ziggio.Csv.Configuration;

namespace Ziggio.Csv;

public class CsvReaderConfiguration : ICsvReaderConfiguration {
  public CsvReaderConfiguration() {
    ContainsHeaderRow = true;
    Delimiter = ",";
    IgnoreRowsWithErrors = true;
    IsCommaQuoteDelimited = true;
    NewLine = Environment.NewLine;
    QuoteCharacter = "\"";
    RemoveFieldQuotes = true;
    FieldValueRegx = IsCommaQuoteDelimited ? Constants.RegEx.FieldValues(QuoteCharacter) : Constants.RegExCheat.FieldValue;
  }

  public CsvReaderConfiguration(
    IOptions<CsvConfigSection> options
  ) {
    ContainsHeaderRow = options.Value.CsvReader.ContainsHeaderRow;
    Delimiter = options.Value.CsvReader.Delimiter;
    IgnoreRowsWithErrors = options.Value.CsvReader.IgnoreRowsWithErrors;
    IsCommaQuoteDelimited = options.Value.CsvReader.IsCommaQuoteDelimited;
    NewLine = options.Value.CsvReader.NewLine;
    QuoteCharacter = options.Value.CsvReader.QuoteCharacter;
    RemoveFieldQuotes = options.Value.CsvReader.RemoveFieldQuotes;
    FieldValueRegx = IsCommaQuoteDelimited ? Constants.RegEx.FieldValues(QuoteCharacter) : Constants.RegExCheat.FieldValue;
  }

  public bool ContainsHeaderRow { get; }
  public string Delimiter { get; }
  public string FieldValueRegx { get; private set; }
  public bool IgnoreRowsWithErrors { get; }
  public bool IsCommaQuoteDelimited { get; }
  public string NewLine { get; }
  public string QuoteCharacter { get; }
  public bool RemoveFieldQuotes { get; }

  public void Validate() {
    if (!Regex.IsMatch(QuoteCharacter, Constants.RegExCheat.MatchStart + Constants.RegExCheat.FieldQuote + Constants.RegExCheat.MatchEnd)) {
      throw new Exception("Invalid quote character in configuration");
    }
  }
}