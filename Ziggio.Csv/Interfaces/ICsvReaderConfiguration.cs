namespace Ziggio.Csv;

public interface ICsvReaderConfiguration {
  bool ContainsHeaderRow { get; }
  string Delimiter { get; }
  string FieldValueRegx { get; }
  bool IgnoreRowsWithErrors { get; }
  bool IsCommaQuoteDelimited { get; }
  string NewLine { get; }
  string QuoteCharacter { get; }
  bool RemoveFieldQuotes { get; }

  void Validate();
}
