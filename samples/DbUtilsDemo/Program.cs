using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;

namespace DbUtilsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EqDemoDb07;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var connection = new SqlConnection(connectionString);
            //ExportTable(connection, "Customers");
            ExportDb(connection);
        }


        static void ExportTable(DbConnection connection, string tableName)
        {
            CheckConnection(connection);

            using (var customersReader = GetDataReader(connection, tableName))
            using (var outXmlFile = new FileStream("Customers.xml", FileMode.Create)) {
                var exporter = new Korzh.DbUtils.Export.XmlDatasetExporter();

                Console.WriteLine($"Exporting {tableName}...");
                exporter.Export(customersReader, outXmlFile);
                Console.WriteLine($"Done!");
            }
        }

        static void ExportDb(DbConnection connection)
        {
            CheckConnection(connection);
            //var datasetExporter = new Korzh.DbUtils.Export.XmlDatasetExporter();
            var datasetExporter = new Korzh.DbUtils.Export.JsonDatasetExporter();
            var bridge = new Korzh.DbUtils.DbBridges.MsSqlBridge(connection as SqlConnection);
            //var packer = new Korzh.DbUtils.Packers.FileFolderPacker("Data");
            var packer = new Korzh.DbUtils.Packers.ZipArchivePacker("EqDemoDb.zip");

            var exporter = new Korzh.DbUtils.Export.DbExporter(bridge, datasetExporter, packer);

            Console.WriteLine($"Exporting database...");
            exporter.Export();
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
    }
}
