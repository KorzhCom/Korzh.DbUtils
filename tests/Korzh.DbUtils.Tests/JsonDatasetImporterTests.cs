using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using FluentAssertions;

using Korzh.DbUtils.Import;

namespace Korzh.DbUtils.Tests
{
    public class JsonDatasetImporterTests
    {

        private readonly JsonDatasetImporter _importer;

        public JsonDatasetImporterTests()
        {
            _importer = new JsonDatasetImporter();
        }


        [Fact]
        public void FileExtentionTest_MustBeJson()
        {
            _importer.FileExtension.Should().Be("json");
        }

        [Fact]
        public void Import_MustBeEqualToTestData()
        {
            var stream = TestUtils.GetResourceStream("Resources", "test-data.json");
            var dataset = _importer.StartImport(stream);
            dataset.Name.Should().Be("Test");

            var records = new List<IDataRecord>();
            while (_importer.HasRecords()) {
                records.Add(_importer.NextRecord());
            }

            _importer.FinishImport();

            var testData = Common.GetTestData();

            records.Should().HaveCount(testData.Count);

            foreach (var item in testData) {
                var record = records.FirstOrDefault(rec => item.Name == (string)rec["Name"] 
                                            && item.Price == (int)rec["Price"]
                                            && item.URL == (string)rec["URL"]);

                record.Should().NotBeNull();
                ((byte[])record["Binary"]).Should().BeEquivalentTo(item.Binary);
            }
        }

    }
}
