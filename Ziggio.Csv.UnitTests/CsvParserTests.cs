using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using RegExConstants = Ziggio.Csv.Constants;
using TestConstants = Ziggio.Csv.UnitTests.Constants;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvParserTests {
  private Mock<ICsvConfiguration> _mockConfiguration;

  [TestInitialize]
  public void Initialize() {
    _mockConfiguration = new Mock<ICsvConfiguration>();
    _mockConfiguration.Setup(config => config.ContainsHeaderRow).Returns(true);
    _mockConfiguration.Setup(config => config.Delimiter).Returns(",");
    _mockConfiguration.Setup(config => config.FieldValueRegx).Returns(RegExConstants.RegEx.FieldValues(RegExConstants.RegExCheat.DoubleQuote));
    _mockConfiguration.Setup(config => config.IsCommaQuoteDelimited).Returns(true);
    _mockConfiguration.Setup(config => config.NewLine).Returns(Environment.NewLine);
    _mockConfiguration.Setup(config => config.QuoteCharacter).Returns("\"");
    _mockConfiguration.Setup(config => config.RemoveFieldQuotes).Returns(true);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.CsvParserMixedData)]
  [DataRow(TestConstants.TestFiles.Food)]
  [DataRow(TestConstants.TestFiles.MeasureUnitNoHeaders)]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders)]
  public void CsvParser_CanOpenFiles(string file) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    using var fileStream = fileInfo.OpenRead();

    fileStream.CanRead.Should().BeTrue();
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders)]
  public void CsvParser_CanParseHeaders(string file) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvParser = new CsvParser(_mockConfiguration.Object, streamReader);

    csvParser.Parse();

    csvParser.Headers.Length.Should().Be(2);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders)]
  public void CsvParser_HeadersCannotBeChanged(string file) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvParser = new CsvParser(_mockConfiguration.Object, streamReader);

    csvParser.Parse();

    csvParser.Headers.Length.Should().Be(2);

    csvParser.Headers.Add("3");

    csvParser.Headers.Length.Should().Be(2);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.CsvParserMixedData, 1, "\"1105905\",\"branded_food\",\"SWANSON BROTH BEEF\",\"\",\"2020-11-13\"")]
  [DataRow(TestConstants.TestFiles.CsvParserMixedData, 84396, "\"372849\",\"branded_food\",\"EL SERRANITO, COOKED HAM\",\"\",\"2019-04-01\"")]
  [DataRow(TestConstants.TestFiles.CsvParserMixedData, 99998, "\"388505\",\"branded_food\",\"MORINAGA, MORI-NU, ORGANIC FIRM SILKEN TOFU\",\"\",\"2019-04-01\"")]
  public void CsvParser_Parse_WithHeaders_MatchExpected(string file, int expectedIndex, string expectedLine) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvParser = new CsvParser(_mockConfiguration.Object, streamReader);

    csvParser.Parse();

    var line = csvParser[expectedIndex];

    line.Should().NotBeNullOrEmpty();
    line.Should().BeEquivalentTo(expectedLine);
  }
}
