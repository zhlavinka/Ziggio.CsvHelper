using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Ziggio.Csv.Configuration;
using Ziggio.Csv.Exceptions;

using RegExConstants = Ziggio.Csv.Constants;
using TestConstants = Ziggio.Csv.UnitTests.Constants;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvConfigurationTests {
  private IOptions<CsvConfigSection> _options;
  private CsvConfiguration _configuration;
  private CsvParser _parser;
  private CsvReader _reader;

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, true, new string[] { "1000", "cup" })]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, false, new string[] { "\"1000\"", "\"cup\"" })]
  public void Configuration_RemoveFieldQuotes(string file, bool removeFieldQuotes, string[] expectedValues) {
    // arrange
    SetupOptions(removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);

    // act
    var values = _reader.GetValues();

    values.Length.Should().Be(expectedValues.Length);

    // assert
    for (int i = 0; i < values.Length; i++) {
      values[i].Should().BeEquivalentTo(expectedValues[i]);
    }
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, new string[] { "id", "name" })]
  [DataRow(TestConstants.TestFiles.MeasureUnitNoHeaders, new string[] { })]
  public void Configuration_ContainsHeaders(string file, string[] headers) {
    // arrange
    SetupOptions(containsHeaderRow: headers.Length > 0);
    SetupCsvParser(file);

    // act
    _parser.Parse();

    // assert
    _parser.Headers.Length.Should().Be(headers.Length);

    if (_parser.Headers.Length > 0) {
      for (int i = 0; i < _parser.Headers.Length; i++) {
        var header = _parser.Headers[i];
        var expected = headers[i];
        header.Should().BeEquivalentTo(expected);
      }
    }
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\"")]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\'")]
  public void Configuration_QuoteCharacter_IsValid(string file, string quoteCharacter) {
    SetupOptions(quoteCharacter: quoteCharacter);
    SetupCsvParser(file);

    _configuration.Invoking(config => config.Validate())
      .Should().NotThrow<Exception>();
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\"\"")]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\'\"")]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "!")]
  public void Configuration_QuoteCharacter_IsInvalid(string file, string quoteCharacter) {
    SetupOptions(quoteCharacter: quoteCharacter);
    SetupCsvParser(file);

    _configuration.Invoking(config => config.Validate())
      .Should().Throw<Exception>();
  }

  [TestMethod]
  [DataRow("RAVIOLI, CHEESE \"\"\"\"DELICATO\"\"\"\", CHEESE \"\"\"\"DELICATO\"\"\"\"", "\"\"\"", "RAVIOLI, CHEESE \"DELICATO\", CHEESE \"DELICATO\"")]
  public void Configuration_EscapeSequence_IsValid(string input, string escapeSequence, string expected) {
    var sanitized = Regex.Replace(input, escapeSequence, string.Empty);
    sanitized.Should().BeEquivalentTo(expected);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.Food)]
  public void Configuration_ThrowErrorOnBadData(string file) {
    SetupOptions(throwErrorOnBadData: true);
    SetupCsvParser(file);

    _parser.Invoking(parser => parser.Parse())
      .Should().Throw<BadDataException>();

    _parser.ErrorRows.Count.Should().BeGreaterThan(0);
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.Food)]
  public void Configuration_DoNotThrowErrorOnBadData(string file) {
    SetupOptions(throwErrorOnBadData: false);
    SetupCsvParser(file);

    _parser.Invoking(parser => parser.Parse())
      .Should().NotThrow<BadDataException>();

    _parser.ErrorRows.Count.Should().BeGreaterThan(0);
  }

  [TestCleanup]
  public void Cleanup() {
    _parser?.Dispose();
    _reader?.Dispose();
  }

  private void SetupCsvParser(string file) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

    var fileStream = fileInfo.OpenRead();
    var streamReader = new StreamReader(fileStream);

    _parser = new CsvParser(_configuration, streamReader);
  }

  private void SetupCsvReader(string file) {
    var fileInfo = TestConstants.TestFiles.GetFileInfo(file);

    var fileStream = fileInfo.OpenRead();
    var streamReader = new StreamReader(fileStream);

    _reader = new CsvReader(_configuration, streamReader);
  }

  private void SetupOptions(
    bool containsHeaderRow = true,
    string delimiter = ",",
    bool ignoreBadData = true,
    bool isCommaQuoteDelimited = true,
    string newLine = @"\r\n",
    string quoteCharacter = "\"",
    bool removeFieldQuotes = false,
    bool throwErrorOnBadData = true
  ) {
    _options = Options.Create(new CsvConfigSection {
      CsvParser = new CsvConfigSection.CsvParserConfigSection {
        ContainsHeaderRow = containsHeaderRow,
        Delimiter = delimiter,
        IgnoreBadData = ignoreBadData,
        IsCommaQuoteDelimited = isCommaQuoteDelimited,
        NewLine = newLine,
        QuoteCharacter = quoteCharacter,
        RemoveFieldQuotes = removeFieldQuotes,
        ThrowErrorOnBadData = throwErrorOnBadData
      }
    });

    _configuration = new CsvConfiguration(_options);
  }
}