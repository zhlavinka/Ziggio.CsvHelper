namespace Ziggio.CsvHelper;

public interface ICsvReaderConfiguration
{
    bool ContainsHeaderRow { get; }
    string Delimiter { get; }
    string NewLine { get; }
    string QuoteCharacter { get; }
    bool RemoveFieldQuotes { get; }
}
