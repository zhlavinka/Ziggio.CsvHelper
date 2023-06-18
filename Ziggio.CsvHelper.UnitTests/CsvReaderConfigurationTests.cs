using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ziggio.CsvHelper.Configuration;

namespace Ziggio.CsvHelper.UnitTests;

[TestClass]
public class CsvReaderConfigurationTests {
  private IOptions<CsvHelperConfigSection> _options;
  private CsvReaderConfiguration _configuration;
  private CsvReader _csvReader;

  [TestMethod]
  [DataRow("measure_unit_w_header_row.csv", true, "1000,cup")]
  [DataRow("measure_unit_w_header_row.csv", false, "\"1000\",\"cup\"")]
  public void Configuration_RemoveFieldQuotes(string file, bool removeFieldQuotes, string expectedLine) {
    // arrange
    SetupOptions(removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);

    // act
    var line = _csvReader.GetLine();

    // assert
    line.Should().BeEquivalentTo(expectedLine);
  }

  [TestMethod]
  [DataRow(
    "measure_unit_w_header_row.csv",
    new string[] { "id", "name" },
    true,
    "1000,cup")]
  [DataRow(
    "measure_unit_w_header_row.csv",
    new string[] { "\"id\"", "\"name\"" },
    false,
    "\"1000\",\"cup\"")]
  [DataRow("measure_unit_no_header_row.csv",
    new string[] { },
    true,
    "1000,cup")]
  [DataRow("measure_unit_no_header_row.csv",
    new string[] { },
    false,
    "\"1000\",\"cup\"")]
  public void Configuration_ContainsHeaders(string file, string[] headers, bool removeFieldQuotes, string expectedLineRead) {
    // arrange
    SetupOptions(containsHeaderRow: headers.Length > 0, removeFieldQuotes: removeFieldQuotes);
    SetupCsvReader(file);

    // act
    var line = _csvReader.GetLine();

    // assert
    line.Should().BeEquivalentTo(expectedLineRead);

    _csvReader.Headers.Length.Should().Be(headers.Length);

    if (headers.Length > 0) {
      for (int i = 0; i < _csvReader.Headers.Length; i++) {
        var header = _csvReader.Headers[i];
        var expected = headers[i];
        header.Should().BeEquivalentTo(expected);
      }
    }
  }

  [TestCleanup]
  public void Cleanup() {
    _csvReader.Dispose();
  }

  private void SetupOptions(
    bool containsHeaderRow = true,
    string delimiter = ",",
    string newLine = @"\r\n",
    string quoteCharacter = "\"",
    bool removeFieldQuotes = false
  ) {
    _options = Options.Create(new CsvHelperConfigSection {
      CsvReader = new CsvHelperConfigSection.CsvReaderConfigSection {
        ContainsHeaderRow = containsHeaderRow,
        Delimiter = delimiter,
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