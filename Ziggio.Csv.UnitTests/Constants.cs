namespace Ziggio.Csv.UnitTests;

public static class Constants {
  public static class TestFiles {
    public const string Food = "food.csv";
    public const string FoodShort = "food_short.csv";
    public const string MeasureUnitNoHeaders = "measure_unit_no_header_row.csv";
    public const string MeasureUnitWithHeaders = "measure_unit_w_header_row.csv";

    public static FileInfo GetFileInfo(string file) {
      return new FileInfo(GetFilePath(file));
    }

    public static string GetFilePath(string file) {
      return Path.Combine(Directory.GetCurrentDirectory(), "Data", file);
    }
  }
}
