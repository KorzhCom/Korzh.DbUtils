using System;
using System.Linq;

using Xunit;

using FluentAssertions;

using Korzh.DbUtils.Loaders;

namespace Korzh.DbUtils.Tests
{
    public class JsonZipFileLoaderTests
    {
        [Theory]
        [InlineData("Orders", 830, 14)]
        [InlineData("Products", 77, 10)]
        [InlineData("Categories", 8, 4)]
        [InlineData("Customers", 91, 11)]
        [InlineData("Employees", 9, 18)]
        public void LoadEntityDataTest(string entityName, int count, int propCount)
        {
            //var zipArhiveStream = TestUtils.GetResourceStream("Resources", "data-json.zip");
            //IDbImporter zipArhiveLoader = new JsonZipFileLoader(zipArhiveStream);

            //var items = zipArhiveLoader.LoadTableData(entityName).ToList();
            //items.Should().HaveCount(count);

            //var item = items.First();
            //item.Properties.Should().HaveCount(propCount);

        }

    }
}
