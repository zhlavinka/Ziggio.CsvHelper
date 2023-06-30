using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Ziggio.Csv.Configuration;

namespace Ziggio.Csv;

public class CsvConfiguration : ICsvConfiguration {
  public CsvConfiguration() {
    ContainsHeaderRow = true;
    Delimiter = ",";
    IgnoreBadData = true;
    IsCommaQuoteDelimited = true;
    NewLine = Environment.NewLine;
    QuoteCharacter = "\"";
    RemoveFieldQuotes = true;
    ThrowErrorOnBadData = true;
    FieldValueRegx = IsCommaQuoteDelimited ? Constants.RegEx.FieldValues(QuoteCharacter) : Constants.RegExCheat.FieldValue;
  }

  public CsvConfiguration(
    IOptions<CsvConfigSection> options
  ) {
    ContainsHeaderRow = options.Value.CsvParser.ContainsHeaderRow;
    Delimiter = options.Value.CsvParser.Delimiter;
    IgnoreBadData = options.Value.CsvParser.IgnoreBadData;
    IsCommaQuoteDelimited = options.Value.CsvParser.IsCommaQuoteDelimited;
    NewLine = options.Value.CsvParser.NewLine;
    QuoteCharacter = options.Value.CsvParser.QuoteCharacter;
    RemoveFieldQuotes = options.Value.CsvParser.RemoveFieldQuotes;
    ThrowErrorOnBadData = options.Value.CsvParser.ThrowErrorOnBadData;
    FieldValueRegx = IsCommaQuoteDelimited ? Constants.RegEx.FieldValues(QuoteCharacter) : Constants.RegExCheat.FieldValue;
  }

  public bool ContainsHeaderRow { get; }
  public string Delimiter { get; }
  public string FieldValueRegx { get; private set; }
  public bool IgnoreBadData { get; private set; }
  public bool IsCommaQuoteDelimited { get; }
  public string NewLine { get; }
  public string QuoteCharacter { get; }
  public bool RemoveFieldQuotes { get; }
  public bool ThrowErrorOnBadData { get; }

  public void Validate() {
    if (!Regex.IsMatch(QuoteCharacter, Constants.RegExCheat.MatchStart + Constants.RegExCheat.FieldQuote + Constants.RegExCheat.MatchEnd)) {
      throw new Exception("Invalid quote character in configuration");
    }
  }
}