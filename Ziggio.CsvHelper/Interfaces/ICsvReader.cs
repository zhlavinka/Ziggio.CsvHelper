using System.Collections.Immutable;

namespace Ziggio.CsvHelper;

public interface ICsvReader : IDisposable {
  ICsvReaderConfiguration Configuration { get; }
  string? CurrentRecord { get; }
  ImmutableArray<string> Headers { get; }

  void ParseHeaders();
  string GetLine();
  T GetRecord<T>() where T : class;
  List<T> GetRecords<T>() where T : class;
  string[] GetValues();
}
