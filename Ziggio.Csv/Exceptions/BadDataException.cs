namespace Ziggio.Csv.Exceptions;

public class BadDataException : Exception {
  public Dictionary<int, string> BadRecords { get; private set; }

  public BadDataException(Dictionary<int, string> badRecords) : base("Bad data records were found while parsing the file") {
    BadRecords = badRecords;
  }
}