using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ziggio.Csv.UnitTests.Models;

using RegExConstants = Ziggio.Csv.Constants;
using TestConstants = Ziggio.Csv.UnitTests.Constants;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class GenericCsvReaderTests {
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
  [DataRow(TestConstants.TestFiles.FoodShort)]
  public void CsvReader_GetRecord_ReturnsInstance(string file) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader<FoodName>(_mockConfiguration.Object, streamReader);

    var food = csvReader.GetRecord();

    food.Should().NotBeNull();
    food.FdcId.Should().Be(1105904);
    food.DataType.Should().BeEquivalentTo("branded_food");
    food.Description.Should().BeEquivalentTo("WESSON Vegetable Oil 1 GAL");
    food.PublicationDate.Should().Be(new DateOnly(2020, 11, 13));
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.FoodShort)]
  public void CsvReader_GetRecord_LoopAll(string file) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader<FoodName>(_mockConfiguration.Object, streamReader);

    FoodName food;
    int numFoods = 0;

    do {
      food = csvReader.GetRecord();

      if (food is not null)
        numFoods += 1;
    }
    while (food is not null);

    // 38 valid records in file, 2 bad
    numFoods.Should().Be(38);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.FoodShort)]
  public void CsvReader_GetRecords_ReadsAll(string file) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader<FoodName>(_mockConfiguration.Object, streamReader);

    var records = csvReader.GetRecords();

    records.Should().NotBeNullOrEmpty();
    records.Count.Should().Be(38);

    //"388487","branded_food","RASPBERRY DELIGHT","","2019-04-01"
    var spotCheck = records[19];

    spotCheck.Should().NotBeNull();
    spotCheck.FdcId.Should().Be(388487);
    spotCheck.DataType.Should().BeEquivalentTo("branded_food");
    spotCheck.Description.Should().BeEquivalentTo("RASPBERRY DELIGHT");
    spotCheck.PublicationDate.Should().Be(new DateOnly(2019, 04, 01));
  }
}
