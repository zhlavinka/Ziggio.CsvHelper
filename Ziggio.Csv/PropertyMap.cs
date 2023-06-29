namespace Ziggio.Csv;

public class PropertyMap {
  public bool Ignore { get; set; }
  public int HeaderIndex { get; set; }
  public string HeaderName { get; set; }
  public string PropertyName { get; set; }
  public Type PropertyType { get; set; }
}
