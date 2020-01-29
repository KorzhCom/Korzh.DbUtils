using System;
using System.IO;
using System.Text;

using Xunit;
using FluentAssertions;

using Korzh.DbUtils.Export;

namespace Korzh.DbUtils.Tests
{
    public class JsonDatasetExporterTests
    {

        private readonly JsonDatasetExporter _exporter;

        public JsonDatasetExporterTests()
        {
            _exporter = new JsonDatasetExporter();
        }

        [Fact]
        public void FileExtentionTest_MustBeJson()
        {
            _exporter.FileExtension.Should().Be("json");
        }

        [Fact]
        public void Export_MustBeEqualToFileValue()
        {

            var testData = Common.GenerateDataTableForTest();

            MemoryStream stream = new MemoryStream();
            _exporter.ExportDataset(testData.CreateDataReader(), stream, new DatasetInfo("Test", "Test"));

            var result = Encoding.UTF8.GetString(stream.ToArray());
            var trueResult = TestUtils.GetResourceAsString(typeof(JsonDatasetImporterTests).Assembly, "Resources", "test-data.json");

            result.Should().Be(trueResult);
        }

    }
}
