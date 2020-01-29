using System;
using Microsoft.Data.Sqlite;

using Xunit;
using FluentAssertions;

using Korzh.DbUtils.Tests;
using System.Collections.Generic;

namespace Korzh.DbUtils.Sqlite.Tests
{
    public class SqliteBridgeTests
    {
        [Fact]
        public void SeedDataBaseTest_MustFillDataBase()
        {
            var connection = new SqliteConnection("Data Source=:memory:;");
            connection.DefaultTimeout = 100;

            connection.Open();

            //create in-memory test db
            using (var command = connection.CreateCommand()) {
                command.CommandText = TestUtils.GetResourceAsString(typeof(SqliteBridgeTests).Assembly, "Resources", "db.sql");

                command.ExecuteNonQuery();
            }

            DbInitializer.Create(options => {
                options.UseSqlite(connection);
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

            foreach (var tableCount in tableCounts) {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT COUNT(*) FROM {tableCount.Key};";

                    var count = (long)command.ExecuteScalar();

                    count.Should().Be(tableCount.Value, "Wrong number records in {0} table.", tableCount.Key);
                }
            }
               
        }
    }
}
