using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ziggio.Csv.UnitTests.Models;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class GenericCsvReaderTests {
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
  public void CsvReader_FirstGetRecord_ReturnsInstance(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);

    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader<MeasureUnit>(_mockConfiguration.Object, streamReader);

    var measureUnit = csvReader.GetRecord();

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
    using var csvReader = new CsvReader<MeasureUnit>(_mockConfiguration.Object, streamReader);

    var records = csvReader.GetRecords();

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

// needed tests
// - a csv record with random tab or new line character in middle