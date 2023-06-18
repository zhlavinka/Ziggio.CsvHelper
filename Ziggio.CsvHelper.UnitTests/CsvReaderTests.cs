using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text.RegularExpressions;
using Ziggio.CsvHelper.Configuration;
using Ziggio.CsvHelper.UnitTests.Models;

namespace Ziggio.CsvHelper.UnitTests;

[TestClass]
public class CsvReaderTests {
  private Mock<ICsvReaderConfiguration> _mockConfiguration;

  [TestInitialize]
  public void Initialize() {
    _mockConfiguration = new Mock<ICsvReaderConfiguration>();
    _mockConfiguration.Setup(config => config.ContainsHeaderRow).Returns(true);
    _mockConfiguration.Setup(config => config.Delimiter).Returns(",");
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
  [DataRow("measure_unit_w_header_row.csv")]
  public void CsvReader_FirstGetValues_ReturnsStringArray(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var values = csvReader.GetValues();

    values.Length.Should().Be(2);
    values[0].Should().Be("1000");
    values[1].Should().Be("cup");
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv")]
  public void CsvReader_FirstGetRecord_ReturnsInstance(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var measureUnit = csvReader.GetRecord<MeasureUnit>();

    measureUnit.Should().NotBeNull();
    measureUnit.MeasureUnitId.Should().Be(1000);
    measureUnit.Name.Should().BeEquivalentTo("cup");
    measureUnit.Abbreviation.Should().BeEquivalentTo(default);
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv")]
  public void CsvReader_ReadAllRecords(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var records = csvReader.GetRecords<MeasureUnit>();

    records.Should().NotBeNullOrEmpty();
    records.Count.Should().Be(122);

    //"1118","Tablespoons"
    var spotCheck = records[118];

    spotCheck.Should().NotBeNull();
    spotCheck.MeasureUnitId.Should().Be(1118);
    spotCheck.Name.Should().BeEquivalentTo("Tablespoons");
    spotCheck.Abbreviation.Should().BeEquivalentTo(default); 
  }
}