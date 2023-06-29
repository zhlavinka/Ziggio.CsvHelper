namespace Ziggio.Csv;

public interface ICsvParser : IDisposable {
  Dictionary<int, string> ErrorRows { get; }
  Dictionary<int, string> ValidRows { get; }
  void Parse();
}
