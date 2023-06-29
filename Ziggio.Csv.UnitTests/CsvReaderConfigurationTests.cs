using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Ziggio.Csv.Configuration;

namespace Ziggio.Csv.UnitTests;

[TestClass]
public class CsvReaderConfigurationTests {
  private IOptions<CsvConfigSection> _options;
  private CsvReaderConfiguration _configuration;
  private CsvReader _csvReader;

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", true, "1000,cup")]
  [DataRow("measure_unit_w_header_row.csv", false, "\"1000\",\"cup\"")]
  public void Configuration_RemoveFieldQuotes(string file, bool removeFieldQuotes, string expectedValues) {
    // arrange
    SetupOptions(removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);

    // act
    var values = _csvReader.GetValues();
    var expected = expectedValues.Split(',');

    // assert
    for (int i = 0; i < values.Length; i++) {
      values[i].Should().BeEquivalentTo(expected[i]);
    }
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", new string[] { "id", "name" }, true)]
  [DataRow("measure_unit_w_header_row.csv", new string[] { "\"id\"", "\"name\"" }, false)]
  [DataRow("measure_unit_no_header_row.csv", new string[] { }, true)]
  [DataRow("measure_unit_no_header_row.csv", new string[] { }, false)]
  public void Configuration_ContainsHeaders(string file, string[] headers, bool removeFieldQuotes) {
    // arrange
    SetupOptions(containsHeaderRow: headers.Length > 0, removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);

    // act
    _csvReader.GetLine();

    // assert
    _csvReader.Headers.Length.Should().Be(headers.Length);

    if (headers.Length > 0) {
      for (int i = 0; i < _csvReader.Headers.Length; i++) {
        var header = _csvReader.Headers[i];
        var expected = headers[i];
        header.Should().BeEquivalentTo(expected);
      }
    }
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", "\"")]
  [DataRow("measure_unit_w_header_row.csv", "\'")]
  public void Configuration_QuoteCharacter_IsValid(string file, string quoteCharacter) {
    SetupOptions(quoteCharacter: quoteCharacter);
    SetupCsvReader(file);

    _configuration.Invoking(config => config.Validate())
      .Should().NotThrow<Exception>();
  }

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", "\"\"")]
  [DataRow("measure_unit_w_header_row.csv", "\'\"")]
  [DataRow("measure_unit_w_header_row.csv", "!")]
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
    _csvReader?.Dispose();
  }

  private void SetupOptions(
    bool containsHeaderRow = true,
    string delimiter = ",",
    string escapeSequence = "",
    bool isCommaQuoteDelimited = true,
    string newLine = @"\r\n",
    string quoteCharacter = "\"",
    bool removeFieldQuotes = false
  ) {
    _options = Options.Create(new CsvConfigSection {
      CsvReader = new CsvConfigSection.CsvReaderConfigSection {
        ContainsHeaderRow = containsHeaderRow,
        Delimiter = delimiter,
        IsCommaQuoteDelimited = isCommaQuoteDelimited,
        NewLine = newLine,
        QuoteCharacter = quoteCharacter,
        RemoveFieldQuotes = removeFieldQuotes
      }
    });

    _configuration = new CsvReaderConfiguration(_options);
  }

  private void SetupCsvReader(string file) {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file);
    var fileInfo = new FileInfo(filePath);

    fileInfo.Exists.Should().BeTrue();

    var fileStream = fileInfo.OpenRead();
    var streamReader = new StreamReader(fileStream);

    _csvReader = new CsvReader(_configuration, streamReader);
  }
}