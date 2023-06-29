namespace Ziggio.Csv.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public class CsvNameAttribute : Attribute {
  public string Name { get; set; }

  public CsvNameAttribute(
    string name
  ){
    Name = name;
  }
}
