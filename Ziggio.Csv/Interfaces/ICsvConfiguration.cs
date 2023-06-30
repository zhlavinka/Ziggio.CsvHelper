namespace Ziggio.Csv;

public interface ICsvConfiguration {
  bool ContainsHeaderRow { get; }
  string Delimiter { get; }
  string FieldValueRegx { get; }
  bool IgnoreBadData { get; }
  bool IsCommaQuoteDelimited { get; }
  string NewLine { get; }
  string QuoteCharacter { get; }
  bool RemoveFieldQuotes { get; }
  bool ThrowErrorOnBadData { get; }

  void Validate();
}
