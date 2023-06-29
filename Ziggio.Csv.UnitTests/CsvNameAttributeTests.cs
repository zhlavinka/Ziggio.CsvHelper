using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ziggio.Csv.UnitTests.Models;

using RegExConstants = Ziggio.Csv.Constants;
using TestConstants = Ziggio.Csv.UnitTests.Constants;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvNameAttributeTests {
  private Mock<ICsvConfiguration> _mockConfiguration;

  private FoodName _expectedFirstRecord => new FoodName {
    FdcId = 1105904,
    DataType = "branded_food",
    Description = "WESSON Vegetable Oil 1 GAL",
    FoodCategoryId = null,
    PublicationDate = new DateOnly(2020, 11, 13)
  };

  private FoodName _expectedLastRecord => new FoodName {
    FdcId = 388505,
    DataType = "branded_food",
    Description = "MORINAGA, MORI-NU, ORGANIC FIRM SILKEN TOFU",
    FoodCategoryId = null,
    PublicationDate = new DateOnly(2019, 04, 01)
  };

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
  [DataRow(TestConstants.TestFiles.Food)]
  public void CsvReader_GetRecord_UsesCsvNameAttributeCorrectly(string file) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader<FoodName>(_mockConfiguration.Object, streamReader);

    var food = csvReader.GetRecord();

    food.Should().NotBeNull();
    food.FdcId.Should().Be(_expectedFirstRecord.FdcId);
    food.DataType.Should().Be(_expectedFirstRecord.DataType);
    food.Description.Should().Be(_expectedFirstRecord.Description);
    food.FoodCategoryId.Should().BeNull();
    food.PublicationDate.Should().Be(_expectedFirstRecord.PublicationDate);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.Food)]
  public void CsvReader_GetRecords_UsesCsvNameAttributeCorrectly(string file) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    using var fileStream = fileInfo.OpenRead();
    using var streamReader = new StreamReader(fileStream);
    using var csvReader = new CsvReader<FoodName>(_mockConfiguration.Object, streamReader);

    var foods = csvReader.GetRecords();

    var first = foods.First();
    first.Should().NotBeNull();
    first.FdcId.Should().Be(_expectedFirstRecord.FdcId);
    first.DataType.Should().Be(_expectedFirstRecord.DataType);
    first.Description.Should().Be(_expectedFirstRecord.Description);
    first.FoodCategoryId.Should().BeNull();
    first.PublicationDate.Should().Be(_expectedFirstRecord.PublicationDate);

    var last = foods.Last();
    last.Should().NotBeNull();
    last.FdcId.Should().Be(_expectedLastRecord.FdcId);
    last.DataType.Should().Be(_expectedLastRecord.DataType);
    last.Description.Should().Be(_expectedLastRecord.Description);
    last.FoodCategoryId.Should().BeNull();
    last.PublicationDate.Should().Be(_expectedLastRecord.PublicationDate);
  }
}
