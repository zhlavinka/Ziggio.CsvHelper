using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Ziggio.CsvHelper;

public class CsvReader : ICsvReader, IDisposable {
  private readonly ICsvReaderConfiguration _configuration;
  private ImmutableArray<string> _headers;
  private readonly StreamReader _reader;

  public CsvReader(
    ICsvReaderConfiguration configuration,
    StreamReader reader) {
    _reader = reader;
    _configuration = configuration;

    _headers = ImmutableArray.Create<string>();
  }

  public ICsvReaderConfiguration Configuration => _configuration;

  public string? CurrentRecord { get; private set; }

  public ImmutableArray<string> Headers => _headers;

  public void Dispose() {
    _reader.Dispose();
  }

  private bool CheckHeaders() {
    if (Configuration.ContainsHeaderRow
      && _headers.Length == 0) {
      ParseHeaders();
    }
    return !Configuration.ContainsHeaderRow
      || (Configuration.ContainsHeaderRow && _headers.Length > 0);
  }

  public void ParseHeaders() {
    if (Configuration.ContainsHeaderRow
      && _headers.Length == 0) {
      var headerLine = _reader.ReadLine();
      var regx = "\"(.*?)\"";
      var matches = Regex.Matches(headerLine, regx);

      var headers = new string[matches.Count];
      for (int i = 0; i < matches.Count; i++) {
        var header = Sanitize(matches[i].Value);
        headers[i] = header;
      }

      _headers = ImmutableArray.Create(headers);
    }
  }

  public string GetLine() {
    if (!Read())
      return null;

    return CurrentRecord;
  }

  public T GetRecord<T>() where T : class {
    var values = GetValues();

    if (values is null)
      return default(T);

    var instance = Activator.CreateInstance(typeof(T));

    var properties = typeof(T).GetProperties();
    for (int i = 0; i < properties.Length; i++) {
      var property = properties[i];
      object value = default;

      if (i < values.Length) {
        value = StringToPrimitive(property.PropertyType, values[i]);
      }

      properties[i].SetValue(instance, value);
    }

    return (T)instance!;
  }

  public List<T> GetRecords<T>() where T : class {
    var records = new List<T>();

    var record = GetRecord<T>();
    do {
      records.Add(record);
      record = GetRecord<T>();
    } while (record != null);

    return records;
  }

  public string[] GetValues() {
    if (!Read())
      return null;

    var splitValues = CurrentRecord.Split(Configuration.Delimiter, StringSplitOptions.RemoveEmptyEntries);

    var values = new List<string>();

    foreach (var splitValue in splitValues) {
      var sanitized = Sanitize(splitValue);
      values.Add(sanitized);
    }

    return values.ToArray();
  }

  private bool Read() {
    if (!CheckHeaders())
      throw new Exception("Error Parsing Headers");

    // get next line
    var line = _reader.ReadLine();

    if (string.IsNullOrEmpty(line)) {
      CurrentRecord = null;
      return false;
    }

    // remove white space
    line = Regex.Replace(line, Constants.RegEx.AllWhitespace, string.Empty);

    // check configuration
    if (_configuration.RemoveFieldQuotes) {
      line = Regex.Replace(line, Constants.RegEx.FieldQuote, string.Empty);
    }

    CurrentRecord = line;

    return true;
  }

  private object StringToPrimitive(Type type, string value) {
    switch (type) {

      case Type boolType when boolType == typeof(bool):
        return Convert.ToBoolean(value);

      case Type byteType when byteType == typeof(byte):
        return Convert.ToByte(value);

      case Type sByteType when sByteType == typeof(sbyte):
        return Convert.ToSByte(value);

      case Type charType when charType == typeof(char):
        return Convert.ToChar(value);

      case Type decimalType when decimalType == typeof(decimal):
        return Convert.ToDecimal(value);

      case Type doubleType when doubleType == typeof(double):
        return Convert.ToDouble(value);

      case Type singleType when singleType == typeof(float):
        return Convert.ToSingle(value);

      case Type intType when intType == typeof(int):
        return Convert.ToInt32(value);

      case Type intType when intType == typeof(uint):
        return Convert.ToUInt32(value);

      case Type longType when longType == typeof(long):
        return Convert.ToInt64(value);

      case Type uLongType when uLongType == typeof(ulong):
        return Convert.ToUInt64(value);

      case Type shortType when shortType == typeof(short):
        return Convert.ToInt16(value);

      case Type uShortType when uShortType == typeof(ushort):
        return Convert.ToUInt16(value);

      default:
        return value;
    }
  }

  private string Sanitize(string value) {
    if (Configuration.RemoveFieldQuotes
      && Regex.IsMatch(value, Constants.RegEx.FieldQuote + Constants.RegEx.FieldValue + Constants.RegEx.FieldQuote)) {
      return Regex.Replace(value, Constants.RegEx.FieldQuote, string.Empty);
    }
    return value;
  }
}