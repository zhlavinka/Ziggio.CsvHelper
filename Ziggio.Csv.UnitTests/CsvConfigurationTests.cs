using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Ziggio.Csv.Configuration;

using RegExConstants = Ziggio.Csv.Constants;
using TestConstants = Ziggio.Csv.UnitTests.Constants;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvConfigurationTests {
  private IOptions<CsvConfigSection> _options;
  private CsvConfiguration _configuration;
  private CsvReader _reader;

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, true, "1000,cup")]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, false, "\"1000\",\"cup\"")]
  public void Configuration_RemoveFieldQuotes(string file, bool removeFieldQuotes, string expectedValues) {
    // arrange
    SetupOptions(removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);

    // act
    var values = _reader.GetValues();
    var expected = expectedValues.Split(',');

    // assert
    for (int i = 0; i < values.Length; i++) {
      values[i].Should().BeEquivalentTo(expected[i]);
    }
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, new string[] { "id", "name" }, true)]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, new string[] { "\"id\"", "\"name\"" }, false)]
  [DataRow(TestConstants.TestFiles.MeasureUnitNoHeaders, new string[] { }, true)]
  [DataRow(TestConstants.TestFiles.MeasureUnitNoHeaders, new string[] { }, false)]
  public void Configuration_ContainsHeaders(string file, string[] headers, bool removeFieldQuotes) {
    // arrange
    SetupOptions(containsHeaderRow: headers.Length > 0, removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);
    
    // act
    _reader.GetLine();

    // assert
    _reader.Headers.Length.Should().Be(headers.Length);

    if (headers.Length > 0) {
      for (int i = 0; i < _reader.Headers.Length; i++) {
        var header = _reader.Headers[i];
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
    SetupCsvReader(file);

    _configuration.Invoking(config => config.Validate())
      .Should().NotThrow<Exception>();
  }

  [TestMethod]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\"\"")]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "\'\"")]
  [DataRow(TestConstants.TestFiles.MeasureUnitWithHeaders, "!")]
  public void Configuration_QuoteCharacter_IsInvalid(string file, string quoteCharacter) {
    SetupOptions(quoteCharacter: quoteCharacter);
    SetupCsvReader(file);

    _configuration.Invoking(config => config.Validate())
      .Should().Throw<Exception>();
  }

  [TestMethod]
  [DataRow("RAVIOLI, CHEESE \"\"\"\"DELICATO\"\"\"\", CHEESE \"\"\"\"DELICATO\"\"\"\"", "\"\"\"", "RAVIOLI, CHEESE \"DELICATO\", CHEESE \"DELICATO\"")]
  public void Configuration_EscapeSequence_IsValid(string input, string escapeSequence, string expected) {
    var sanitized = Regex.Replace(input, escapeSequence, string.Empty);
    sanitized.Should().BeEquivalentTo(expected);
  }

  [TestCleanup]
  public void Cleanup() {
    _reader?.Dispose();
  }

  private void SetupOptions(
    bool containsHeaderRow = true,
    string delimiter = ",",
    bool isCommaQuoteDelimited = true,
    string newLine = @"\r\n",
    string quoteCharacter = "\"",
    bool removeFieldQuotes = false
  ) {
    _options = Options.Create(new CsvConfigSection {
      CsvParser = new CsvConfigSection.CsvParserConfigSection {
        ContainsHeaderRow = containsHeaderRow,
        Delimiter = delimiter,
        IsCommaQuoteDelimited = isCommaQuoteDelimited,
        NewLine = newLine,
        QuoteCharacter = quoteCharacter,
        RemoveFieldQuotes = removeFieldQuotes
      }
    });

    _configuration = new CsvConfiguration(_options);
  }

  private void SetupCsvReader(string file) {
    var fileInfo = new FileInfo(TestConstants.TestFiles.GetFilePath(file));

    var fileStream = fileInfo.OpenRead();
    var streamReader = new StreamReader(fileStream);

    _reader = new CsvReader(_configuration, streamReader);
  }
}