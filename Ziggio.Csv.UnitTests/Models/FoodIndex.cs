using Ziggio.Csv.Attributes;

namespace Ziggio.Csv.UnitTests.Models;

public class FoodIndex {
  [CsvIndex(0)]
  public int FdcId { get; set; }
  public int Id { get; set; }
  [CsvIndex(3)]
  public int FoodCategoryId { get; set; }
  public string Name { get; set; }
  public string ScientificName { get; set; }
  public string Class { get; set; }
  [CsvIndex(1)]
  public string DataType { get; set; }
  [CsvIndex(2)]
  public string Description { get; set; }
  [CsvIndex(4)]
  public DateOnly PublicationDate { get; set; }
  public string FoodKey { get; set; }

  public List<MeasureUnit> MeasureUnits { get; set; }
}
