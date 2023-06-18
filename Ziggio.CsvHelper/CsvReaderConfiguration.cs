using Microsoft.Extensions.Options;
using Ziggio.CsvHelper.Configuration;

namespace Ziggio.CsvHelper;

public class CsvReaderConfiguration : ICsvReaderConfiguration {
  private readonly CsvHelperConfigSection _configuration;

  public CsvReaderConfiguration(
    IOptions<CsvHelperConfigSection> options
  ) {
    _configuration = options.Value;
  }

  public bool ContainsHeaderRow => _configuration.CsvReader.ContainsHeaderRow;
  public string Delimiter => _configuration.CsvReader.Delimiter;
  public string NewLine => _configuration.CsvReader.NewLine;
  public string QuoteCharacter => _configuration.CsvReader.QuoteCharacter;
  public bool RemoveFieldQuotes => _configuration.CsvReader.RemoveFieldQuotes;
}
