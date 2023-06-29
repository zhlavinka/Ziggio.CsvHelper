using System.Collections.Immutable;

namespace Ziggio.Csv;

public interface ICsvReader : IDisposable {
  ICsvConfiguration Configuration { get; }
  ImmutableArray<string> Headers { get; }

  string? GetLine();
  string[] GetValues();
}

public interface ICsvReader<T> : ICsvReader where T : class {
  T GetRecord();
  List<T> GetRecords();
}
