using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Ziggio.Csv.Attributes;
using Ziggio.Csv.Extensions;

namespace Ziggio.Csv;

public class CsvReader : ICsvReader, IDisposable {
  private readonly ICsvReaderConfiguration _configuration;
  private ImmutableArray<string> _headers;
  private readonly StreamReader _reader;
  private bool _isInitialized;

  public CsvReader(
    StreamReader streamReader
  ) : this(new CsvReaderConfiguration(), streamReader) {

  }

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

  public PropertyAtlas PropertyAtlas { get; set; } = new PropertyAtlas();

  public void Dispose() {
    _reader.Dispose();
  }

  public string GetLine() {
    if (!Read())
      return null;

    return CurrentRecord;
  }

  public string[] GetValues() {
    if (!Read())
      return null;

    CurrentRecord = PreMatchSanitize(CurrentRecord);

    var matches = Regex.Matches(CurrentRecord, _configuration.FieldValueRegx);

    var values = new List<string>();

    foreach (Match match in matches) {
      var sanitized = PostMatchSanitize(match.Value);
      values.Add(sanitized);
    }

    return values.ToArray();
  }

  private void Initialize() {
    if (Configuration.ContainsHeaderRow && _headers.Length == 0)
      ParseHeaders();

    if (!PropertyAtlas.IsInitialized)
      SetupPropertyAtlas();

    _isInitialized = true;
  }

  private void ParseHeaders() {
    var headerLine = _reader.ReadLine();
    var matches = Regex.Matches(headerLine, Constants.RegEx.HeaderNames);

    var headers = new string[matches.Count];
    for (int i = 0; i < matches.Count; i++) {
      var header = PostMatchSanitize(matches[i].Value);
      headers[i] = header;
    }

    _headers = ImmutableArray.Create(headers);
  }

  private bool Read() {
    if (!_isInitialized)
      Initialize();

    // get next line
    var line = _reader.ReadLine();

    if (string.IsNullOrEmpty(line)) {
      CurrentRecord = null;
      return false;
    }

    // remove leading/trailing white space
    line = line.Trim();

    CurrentRecord = line;

    return true;
  }

  protected object StringToPrimitive(Type type, string value) {
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

      case Type nullableIntType when nullableIntType == typeof(int?):
        return value.ToNullable<int>();

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

      case Type dateTimeType when dateTimeType == typeof(DateTime):
        return DateTime.Parse(value);

      case Type dateOnlyType when dateOnlyType == typeof(DateOnly):
        return DateOnly.Parse(value);

      default:
        return value;
    }
  }

  private string PreMatchSanitize(string line) {
    return Regex.Replace(CurrentRecord, "(\"){2,}(?!,)", "\"");
  }

  private string PostMatchSanitize(string value) {
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

  private void SetupPropertyAtlas() {
    for (int i = 0; i < _headers.Length; i++) {
      PropertyAtlas.Add(new PropertyMap {
        HeaderIndex = i,
        HeaderName = _headers[i]
      });
    }
  }
}

public class CsvReader<T> : CsvReader, ICsvReader<T> where T : class {
  private bool _isInitialized = false;

  public CsvReader(StreamReader streamReader) : base(streamReader) { }

  public CsvReader(ICsvReaderConfiguration configuration, StreamReader streamReader) : base(configuration, streamReader) { }

  public T GetRecord() {
    var values = GetValues();

    if (values is null)
      return default(T);

    if (!_isInitialized)
      ParseModel();

    var instance = Activator.CreateInstance(typeof(T));

    foreach (var map in PropertyAtlas) {
      // get value from file and convert to type found on model property
      var value = StringToPrimitive(map.PropertyType, values[map.HeaderIndex]);
      // find and set model property value
      instance.GetType().GetProperty(map.PropertyName).SetValue(instance, value);
    }

    return (T)instance!;
  }

  public List<T> GetRecords() {
    var records = new List<T>();

    var record = GetRecord();
    do {
      records.Add(record);
      record = GetRecord();
    } while (record != null);

    return records;
  }

  private void ParseModel() {
    var propsWithCsvNameAttrs = typeof(T).GetProperties().Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(CsvNameAttribute)));

    foreach (var prop in propsWithCsvNameAttrs) {
      var attr = prop.GetCustomAttributes(typeof(CsvNameAttribute), true).First() as CsvNameAttribute;

      PropertyAtlas[attr.Name].PropertyName = prop.Name;
      PropertyAtlas[attr.Name].PropertyType = prop.PropertyType;
    }

    _isInitialized = true;
  }
}