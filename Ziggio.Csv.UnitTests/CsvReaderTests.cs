using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using RegExConstants = Ziggio.Csv.Constants;
using TestConstants = Ziggio.Csv.UnitTests.Constants;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvReaderTests {
  private Mock<ICsvConfiguration> _mockConfiguration;

  [TestInitialize]
  public void Initialize() {
    _mockConfiguration = new Mock<ICsvConfiguration>();
    _mockConfiguration.Setup(config => config.ContainsHeaderRow).Returns(true);
    _mockConfiguration.Setup(config => config.Delimiter).Returns(",");
    _mockConfiguration.Setup(config => config.FieldValueRegx).Returns(RegExConstants.RegEx.FieldValues(RegExConstants.RegExCheat.DoubleQuote));
    _mockConfiguration.Setup(config => config.IgnoreBadData).Returns(true);
    _mockConfiguration.Setup(config => config.IsCommaQuoteDelimited).Returns(true);
    _mockConfiguration.Setup(config => config.NewLine).Returns(Environment.NewLine);
    _mockConfiguration.Setup(config => config.QuoteCharacter).Returns("\"");
    _mockConfiguration.Setup(config => config.RemoveFieldQuotes).Returns(true);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\"1000\",\"cup\"")]
  [DataRow(TestConstants.TestFiles.Food, "\"1105904\",\"branded_food\",\"WESSON Vegetable Oil 1 GAL\",\"\",\"2020-11-13\"")]
  public void CsvReader_GetLine_ReturnsExpectedLine(string file, string expectedLine) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader(_mockConfiguration.Object, streamReader);

    var line = csvReader.GetLine();

    line.Should().BeEquivalentTo(expectedLine);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "1000", "cup")]
  [DataRow(TestConstants.TestFiles.Food, "1105904", "branded_food", "WESSON Vegetable Oil 1 GAL", "", "2020-11-13")]
  public void CsvReader_GetValues_ReturnsStringArray(string file, params string[] expectedValues) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

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