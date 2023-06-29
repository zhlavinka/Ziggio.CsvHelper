namespace Ziggio.Csv.Exceptions;

public class NotParsedException : Exception {
  public NotParsedException() : base($"The file canot be read because it has not been parsed.") {
    
  }
}