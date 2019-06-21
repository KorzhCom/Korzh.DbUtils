using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using FluentAssertions;

using Korzh.DbInitializer.Loaders;

namespace Korzh.DbInitializer.Tests
{
    public class XmlZipFileLoaderTests
    {

        [Theory]
        [InlineData("Orders", 830, 13)]
        [InlineData("Products", 77, 10)]
        [InlineData("Categories", 8, 4)]
        [InlineData("Customers", 91, 10)]
        [InlineData("Employees", 9, 18)]
        public void LoadEntityDataTest(string entityName, int count, int propCount)
        {
            var zipArhiveStream = TestUtils.GetResourceStream("Resources", "data-xml.zip");
            IDbInitializerLoader zipArhiveLoader = new XmlZipFileLoader(zipArhiveStream);

            var items = zipArhiveLoader.LoadEntityData(entityName).ToList();
            items.Should().HaveCount(count);

            var item = items.First();
            item.Properties.Should().HaveCount(propCount);

        }

        [Theory]
        [InlineData("Orders", typeof(Order), 830)]
        [InlineData("Products", typeof(Product), 77)]
        [InlineData("Categories", typeof(Category), 8)]
        [InlineData("Customers", typeof(Customer), 91)]
        [InlineData("Employees", typeof(Employee), 9)]
        public void LoadFromEntityTypeDataTest(string entityName, Type entityType, int count)
        {
            var zipArhiveStream = TestUtils.GetResourceStream("Resources", "data-xml.zip");
            IDbInitializerLoader zipArhiveLoader = new XmlZipFileLoader(zipArhiveStream);

            var items = zipArhiveLoader.LoadEntityData(entityName, entityType).ToList();
            items.Should().HaveCount(count);
        }
    }
}
