using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Ziggio.Csv.Attributes;
using Ziggio.Csv.Extensions;

namespace Ziggio.Csv;

public class CsvReader : ICsvReader {
  private readonly ICsvConfiguration _configuration;
  private readonly ICsvParser _parser;

  private bool _isInitialized;

  public CsvReader(
    StreamReader streamReader) 
    : this(new CsvConfiguration(), streamReader) {

  }

  public CsvReader(
    ICsvConfiguration configuration,
    StreamReader reader) {

    _configuration = configuration;
    _parser = new CsvParser(_configuration, reader);
  }

  public ICsvConfiguration Configuration => _configuration;

  public ImmutableArray<string> Headers => _parser.Headers;

  public PropertyAtlas PropertyAtlas { get; set; } = new PropertyAtlas();

  public void Dispose() {
    _parser?.Dispose();
  }

  public string? GetLine() {
    if (!_isInitialized)
      Initialize();

    return _parser.GetNextLine();
  }

  public string[] GetValues() {
    var line = GetLine();

    line = PreMatchSanitize(line);

    var matches = Regex.Matches(line, _configuration.FieldValueRegx);

    var values = new List<string>();

    foreach (Match match in matches) {
      var sanitized = PostMatchSanitize(match.Value);
      values.Add(sanitized);
    }

    return values.ToArray();
  }

  private void Initialize() {
    if (!_parser.Parsed)
      _parser.Parse();

    if (!PropertyAtlas.IsInitialized)
      SetupPropertyAtlas();

    _isInitialized = true;
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
    return Regex.Replace(line, "(\"){2,}(?!,)", "\"");
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
    for (int i = 0; i < _parser.Headers.Length; i++) {
      PropertyAtlas.Add(new PropertyMap {
        HeaderIndex = i,
        HeaderName = _parser.Headers[i]
      });
    }
  }
}

public class CsvReader<T> : CsvReader, ICsvReader<T> where T : class {
  private bool _isInitialized = false;

  public CsvReader(StreamReader streamReader) : base(streamReader) { }

  public CsvReader(ICsvConfiguration configuration, StreamReader streamReader) : base(configuration, streamReader) { }

  public T GetRecord() {
    var values = GetValues();

    if (values is null || values.Length == 0)
      return default(T);

    if (!_isInitialized)
      Initialize();

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

  private void Initialize() {
    var propsWithCsvNameAttrs = typeof(T).GetProperties().Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(CsvNameAttribute)));

    foreach (var prop in propsWithCsvNameAttrs) {
      var attr = prop.GetCustomAttributes(typeof(CsvNameAttribute), true).First() as CsvNameAttribute;

      PropertyAtlas[attr.Name].PropertyName = prop.Name;
      PropertyAtlas[attr.Name].PropertyType = prop.PropertyType;
    }

    _isInitialized = true;
  }
}
