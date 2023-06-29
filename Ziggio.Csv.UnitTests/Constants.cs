namespace Ziggio.Csv.UnitTests;

public static class Constants {
  public static class TestFiles {
    public const string CsvParserMixedData = "csvparser_mixed_data.csv";
    public const string Food = "food.csv";
    public const string MeasureUnitNoHeaders = "measure_unit_no_header_row.csv";
    public const string MeasureUnitWithHeaders = "measure_unit_w_header_row.csv";

    public static string GetFilePath(string fileName) {
      return Path.Combine(Directory.GetCurrentDirectory(), "Data", fileName);
    }
  }
}
