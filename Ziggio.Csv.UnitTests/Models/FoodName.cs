using Ziggio.Csv.Attributes;

namespace Ziggio.Csv.UnitTests.Models;

public class FoodName {
  [CsvName("fdc_id")]
  public int FdcId { get; set; }
  public int Id { get; set; }
  [CsvName("food_category_id")]
  public int? FoodCategoryId { get; set; }
  public string Name { get; set; }
  public string ScientificName { get; set; }
  public string Class { get; set; }
  [CsvName("data_type")]
  public string DataType { get; set; }
  [CsvName("description")]
  public string Description { get; set; }
  [CsvName("publication_date")]
  public DateOnly PublicationDate { get; set; }
  public string FoodKey { get; set; }

  public List<MeasureUnit> MeasureUnits { get; set; }
}
