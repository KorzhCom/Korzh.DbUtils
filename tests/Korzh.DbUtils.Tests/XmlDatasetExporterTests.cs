using System;
using System.IO;
using System.Text;

using Xunit;
using FluentAssertions;

using Korzh.DbUtils.Export;

namespace Korzh.DbUtils.Tests
{
    public class XmlDatasetExporterTests
    {
        private readonly XmlDatasetExporter  _exporter;

        public XmlDatasetExporterTests()
        {
            _exporter = new XmlDatasetExporter();
        }

        [Fact]
        public void FileExtentionTest_MustXml()
        {
            _exporter.FileExtension.Should().Be("xml");
        }

        [Fact]
        public void Export_MustBeEqualToFileValue()
        {

            var testData = Common.GenerateDataTableForTest();

            MemoryStream stream = new MemoryStream();
            _exporter.ExportDataset(testData.CreateDataReader(), stream, new DatasetInfo("Test", "Test"));

            var result = Encoding.UTF8.GetString(stream.ToArray());
            var trueResult = TestUtils.GetResourceAsString("Resources", "test-data.xml");

            result.Should().Be(trueResult);
        }
    }
}
