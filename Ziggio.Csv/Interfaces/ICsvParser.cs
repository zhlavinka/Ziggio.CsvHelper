using System.Collections.Immutable;

namespace Ziggio.Csv;

public interface ICsvParser : IDisposable {
  string? CurrentLine { get; }
  Dictionary<int, string> ErrorRows { get; }
  ImmutableArray<string> Headers { get; }
  bool Parsed { get; }
  Dictionary<int, string> ValidRows { get; }

  string? GetNextLine();
  void Parse();
}
