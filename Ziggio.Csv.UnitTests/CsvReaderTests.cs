using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvReaderTests {
  private Mock<ICsvReaderConfiguration> _mockConfiguration;

  [TestInitialize]
  public void Initialize() {
    _mockConfiguration = new Mock<ICsvReaderConfiguration>();
    _mockConfiguration.Setup(config => config.ContainsHeaderRow).Returns(true);
    _mockConfiguration.Setup(config => config.Delimiter).Returns(",");
    _mockConfiguration.Setup(config => config.FieldValueRegx).Returns(Constants.RegEx.FieldValues);
    _mockConfiguration.Setup(config => config.IsCommaQuoteDelimited).Returns(true);
    _mockConfiguration.Setup(config => config.NewLine).Returns(Environment.NewLine);
    _mockConfiguration.Setup(config => config.QuoteCharacter).Returns("\"");
    _mockConfiguration.Setup(config => config.RemoveFieldQuotes).Returns(true);
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv")]
  public void CsvReader_CanOpenFiles(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();

    fileStream.CanRead.Should().BeTrue();
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv")]
  public void CsvReader_CanParseHeaders(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var line = csvReader.GetLine();

    csvReader.Headers.Length.Should().Be(2);
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv")]
  public void CsvReader_HeadersCannotBeChanged(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var line = csvReader.GetLine();

    csvReader.Headers.Length.Should().Be(2);

    csvReader.Headers.Add("3");

    csvReader.Headers.Length.Should().Be(2);
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", "1000,cup")]
  public void CsvReader_FirstGetLine_WithHeaders_ShouldMatchExpectedLine(string file, string expectedLine) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var line = csvReader.GetLine();

    line.Should().BeEquivalentTo(expectedLine);
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", "1000", "cup")]
  [DataRow("food.csv", "1105904", "branded_food", "WESSON Vegetable Oil 1 GAL", "", "2020-11-13")]
  public void CsvReader_FirstGetValues_ReturnsStringArray(string file, params string[] expectedValues) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var values = csvReader.GetValues();

    values.Length.Should().Be(expectedValues.Length);
    for (int i = 0; i < expectedValues.Length; i++) {
      values[i].Should().Be(expectedValues[i]);
    }
  }
}