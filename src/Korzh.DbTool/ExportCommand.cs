using System;

using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

using McMaster.Extensions.CommandLineUtils;

using Korzh.DbUtils.Export;
using Korzh.DbUtils.SqlServer;
using Korzh.DbUtils;
using Korzh.DbUtils.Packing;

namespace Korzh.DbTool
{
    public class ExportCommand : ICommand
    {
        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Exports some DB to a backup archive in the specified format (XML, JSON)";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.FormatOption);
            command.Options.Add(options.LocalConfigFilePathOption);

            var connectionArgument = command.Argument("<connection ID>", "The ID of the some previously registered connection");

            command.OnExecute(new ExportCommand(options, connectionArgument.Value).Run);

        }

        private readonly string _connectionId;
        private readonly GlobalOptions _options;

        private DbConnection _connection;

        public ExportCommand(GlobalOptions options, string connectionId)
        {
            _options = options;
            _connectionId = connectionId;
        }

        private void CheckConnection()
        {
            if (_connection == null) {
                //!!!!!!!!!!!!!!! replace with reading connection string from configuration
                var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EqDemoDb07;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                _connection = new SqlConnection(connectionString);
            }

            if (_connection.State != ConnectionState.Open) {
                Console.WriteLine("Openning connection...");
                _connection.Open();
            }
        }

        private IDatasetExporter GetDatasetExporter()
        {
            switch (_options.Format) { 
                case "xml":
                    return new XmlDatasetExporter();
                default:
                    return new JsonDatasetExporter();
            }
        }

        private IDataPacker GetPacker()
        {
            var zipFilePath = "export.zip";
            return new ZipFilePacker(zipFilePath);
        }

        public int Run()
        {
            if (string.IsNullOrEmpty(_connectionId)) {
                Console.WriteLine("No connection is specified");
                return -1;
            }

            CheckConnection();

            var bridge = new SqlServerBridge(_connection as SqlConnection);
            var exporter = new DbExporter(bridge, GetDatasetExporter(), GetPacker());

            Console.WriteLine($"Exporting database...");
            exporter.Export();

            return 0;
        }
    }
}
