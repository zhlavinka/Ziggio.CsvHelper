using System.ComponentModel;

namespace Ziggio.Csv.Extensions;

public static class PrimitiveExtensions {
  public static T? ToNullable<T>(this string s) where T : struct {
    T? result = new T?();
    try {
      if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0) {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        result = (T)converter.ConvertFromString(s);
      }
    }
    catch { }
    return result;
  }
}
