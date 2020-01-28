using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Korzh.DbUtils.Tests;

namespace Korzh.DbUtils.EntityFrameworkCore.InMemory.Tests
{
    public class DbContextBridgeTests
    {
        [Fact]
        public void SeedDataBaseTest_MustFillDataBase()
        {

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase("test-db");

            var dbContext = new AppDbContext(optionsBuilder.Options);
            dbContext.Database.EnsureCreated();

            DbInitializer.Create(options => {
                options.UseInMemoryDatabase(dbContext);
                options.UseJsonImporter();
                options.UseResourceFileUnpacker(typeof(TestUtils).Assembly, "Resources\\Nwind");
            })
            .Seed();

            var tableCounts = new Dictionary<string, int>()
            {
                ["Categories"] = 8,
                ["Customers"] = 91,
                ["Employees"] = 9,
                ["Products"] = 77,
                ["Orders"] = 830,
                ["Order_Details"] = 2155,
                ["Suppliers"] = 29,
                ["Shippers"] = 3
            };

            foreach (var entityType in dbContext.Model.GetEntityTypes()) {

                var tableName = entityType.GetTableName();
                var count = tableCounts[tableName];

                var realCount = dbContext.Set(entityType.ClrType).Count();
                realCount.Should().Be(count, "Wrong number records in {0} table.", tableName);
            }
        }
    }
}
