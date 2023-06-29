using Ziggio.Csv.Attributes;

namespace Ziggio.Csv.UnitTests.Models;

public class MeasureUnit {
  [CsvName("id")]
  public int MeasureUnitId { get; set; }
  [CsvName("name")]
  public string Name { get; set; }
  public string Abbreviation { get; set; }
}
