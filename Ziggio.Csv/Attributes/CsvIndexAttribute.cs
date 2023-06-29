namespace Ziggio.Csv.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public class CsvIndexAttribute : Attribute {
  public int Index { get; set; }

  public CsvIndexAttribute(
    int index
  ) {
    Index = index;
  }
}
