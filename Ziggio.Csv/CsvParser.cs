using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Ziggio.Csv.Exceptions;

namespace Ziggio.Csv;

public class CsvParser : ICsvParser {
  private readonly ICsvConfiguration _configuration;
  private int _currentIndex = -1;
  private ImmutableArray<string> _headers;
  private bool _isInitialized;
  private readonly StreamReader _reader;

  public string? CurrentLine { get; private set; }

  public Dictionary<int, string> ErrorRows { get; private set; } = new Dictionary<int, string>();

  public ImmutableArray<string> Headers => _headers;

  public bool Parsed { get; private set; } = false;

  public Dictionary<int, string> ValidRows { get; private set; } = new Dictionary<int, string>();

  public string this[int index] {
    get {
      if (ValidRows.ContainsKey(index))
        return ValidRows[index];
      else if (ErrorRows.ContainsKey(index))
        return ErrorRows[index];
      else
        return "";
    }
  }

  public CsvParser(
    ICsvConfiguration configuration,
    StreamReader reader) {

    _configuration = configuration;
    _reader = reader;

    _headers = ImmutableArray.Create<string>();
    _isInitialized = false;
  }

  public string? GetNextLine()
  {
    if (!Parsed)
      throw new NotParsedException();

    _currentIndex += 1;

    CurrentLine = this[_currentIndex];

    return CurrentLine;
  }

  private void Initialize() {
    if (_configuration.ContainsHeaderRow && _headers.Length == 0)
      ParseHeaders();

    _isInitialized = true;
  }

  public void Parse() {
    if (!_isInitialized)
      Initialize();

    if (!Parsed) {
      int index = 0;
      while (Read()) {
        if (Regex.IsMatch(CurrentLine, Constants.RegEx.FieldValues(_configuration.QuoteCharacter))) {
          ValidRows.Add(index, CurrentLine);
        }
        else {
          ErrorRows.Add(index, CurrentLine);
        }
        index += 1;
      }

      // reset CurrentLine
      CurrentLine = null;

      // set parsed flag
      Parsed = true;
    }
  }

  private bool Read() {
    // get next line
    var line = _reader.ReadLine();

    if (string.IsNullOrEmpty(line)) {
      CurrentLine = null;
      return false;
    }

    line = line.Trim();

    CurrentLine = line;

    return true;
  }

  private void ParseHeaders() {
    var headerLine = _reader.ReadLine();
    var matches = Regex.Matches(headerLine, Constants.RegEx.HeaderNames);

    var headers = new string[matches.Count];
    for (int i = 0; i < matches.Count; i++) {
      var header = Sanitize(matches[i].Value);
      headers[i] = header;
    }

    _headers = ImmutableArray.Create(headers);
  }

  private string Sanitize(string value) {
    if (_configuration.RemoveFieldQuotes
        && Regex.IsMatch(value, _configuration.FieldValueRegx)) {
      if (value.StartsWith("\"")) {
        value = value.Remove(0, 1);
      }
      if (value.EndsWith("\"")) {
        value = value.Remove(value.Length - 1, 1);
      }
    }
    return value;
  }

  #region IDisposable
  public void Dispose() {
    _reader?.Dispose();
  }
  #endregion
}
