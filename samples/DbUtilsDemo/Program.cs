using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using Korzh.DbUtils;
using Korzh.DbUtils.Import;
using Korzh.DbUtils.Export;
using Korzh.DbUtils.DbBridges;
using Korzh.DbUtils.Packing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DbUtilsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EqDemoDb21;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var connection = new SqlConnection(connectionString);
            //ExportTable(connection, "Customers");
            //ExportImportDb(connection);
            TestInsertIdentityOn(connection);
        }


        static void ExportTable(DbConnection connection, string tableName)
        {
            CheckConnection(connection);

            using (var customersReader = GetDataReader(connection, tableName))
            using (var outXmlFile = new FileStream("Customers.xml", FileMode.Create)) {
                var exporter = new Korzh.DbUtils.Export.XmlDatasetExporter();

                Console.WriteLine($"Exporting {tableName}...");
                exporter.ExportDataset(customersReader, outXmlFile);
                Console.WriteLine($"Done!");
            }
        }

        static void ExportImportDb(DbConnection connection)
        {
            CheckConnection(connection);
            var datasetExporter = new XmlDatasetExporter();
            //var datasetExporter = new JsonDatasetExporter();
            var bridge = new MsSqlBridge(connection as SqlConnection);
            var packer = new FileFolderPacker("Data");
            //var packer = new ZipFilePacker("EqDemoDb.zip");

            var exporter = new DbExporter(bridge, datasetExporter, packer);

            Console.WriteLine($"Exporting database...");
            exporter.Export();

            //var datasetImporter = new JsonDatasetImporter();
            var datasetImporter = new XmlDatasetImporter();
            var importer = new DbImporter(bridge, datasetImporter, packer);
            Console.WriteLine($"Importing database...");
            importer.Import();

            Console.WriteLine($"Done!");
        }

        private static void CheckConnection(DbConnection connection) {
            if (connection.State != ConnectionState.Open) {
                Console.WriteLine("Openning connection...");
                connection.Open();
            }
        }

        private static IDataReader GetDataReader(DbConnection connection, string tableName)
        {
            var command = GetDbCommandForTable(connection, tableName);
            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }

        private static DbCommand GetDbCommandForTable(DbConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM \"" + tableName + "\"";
            command.CommandType = CommandType.Text;

            return command;
        }


        private static AppDbContext _dbContext = null;

        private static AppDbContext GetDbContext(DbConnection connection)
        {
            if (_dbContext == null) {
                var builder = new DbContextOptionsBuilder<AppDbContext>();
                builder.UseSqlServer(connection);

                _dbContext = new AppDbContext(builder.Options);
            }
            return _dbContext;
        }

        private static void TestInsertIdentityOn(DbConnection connection)
        {
            var supplier = new Models.Supplier {
                Id = 1,
                Address = "Bla-bla",
                City = "London",
                CompanyName = "Bla Company",
                ContactName = "John Doe",
                ContactTitle = "MR.",
                Country = "GB",
                HomePage = "https://www.johndoe.com",
                PostalCode = "1111"
            };

            var dbContext = GetDbContext(connection);

            dbContext.Suppliers.Add(supplier);

            //dbContext.SaveChangesWithIdentity(dbContext.Model.FindEntityType(typeof(Models.Supplier)));

            //var fullTableName = "Suppliers";
            ////dbContext.Model.FindEntityType(typeof(Models.Supplier)).Relational().`

            //Console.WriteLine($"Openning the connection...");
            //dbContext.Database.OpenConnection();
            ////DbContext.Database.BeginTransaction(); //not necessary actually

            //foreach (var sequence in dbContext.Model.Relational().Sequences) {
            //    Console.WriteLine($"SEQ: {sequence.Name}: {sequence.ClrType} : {sequence.Schema}");
            //}

            //var baseSql = "SET IDENTITY_INSERT \"" + fullTableName + "\"";

            //Console.WriteLine($"Setting IDENTITY_INSERT ON...");
            //dbContext.Database.ExecuteSqlCommand(baseSql + " ON");
            //Console.WriteLine($"Saving changes...");
            //dbContext.SaveChanges();
            //Console.WriteLine($"Setting IDENTITY_INSERT OFF...");
            //dbContext.Database.ExecuteSqlCommand(baseSql + " OFF");
        }
    }
}
