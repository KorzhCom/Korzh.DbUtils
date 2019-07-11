using System;

using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

using MySql.Data.MySqlClient;

using McMaster.Extensions.CommandLineUtils;

using Korzh.DbUtils;
using Korzh.DbUtils.Packing;
using Korzh.DbUtils.Export;
using Korzh.DbUtils.SqlServer;
using Korzh.DbUtils.MySql;

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

            var connectionArgument = command.Argument("<connection ID>", "The ID of some previously registered connection")
                                            .IsRequired();

            command.OnExecute(new ExportCommand(connectionArgument, options).Run);

        }

        private readonly CommandArgument _connectionIdArg;
        private readonly GlobalOptions _options;

        private DbConnection _connection;

        public ExportCommand(CommandArgument connectionIdArg, GlobalOptions options)
        {
            _options = options;
            _connectionIdArg = connectionIdArg;
        }

        private void InitConnection(ConnectionInfo info)
        {

            switch (info.DbType) {
                case DbType.SqlServer:
                    _connection = new SqlConnection(info.ConnectionString);
                    break;
                case DbType.MySql:
                    _connection = new MySqlConnection(info.ConnectionString);
                    break;
                default:
                    throw new Exception("Unknown connection type: " + info.DbType);
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

        private IDbReader GetDbReader()
        {
            if (_connection is SqlConnection) {
                return new SqlServerBridge(_connection as SqlConnection);
            }
            else if (_connection is MySqlConnection){
                return new MySqlBride(_connection as MySqlConnection);
            }

            return null;

        }

        private IDataPacker GetPacker()
        {
            var zipFilePath = "export.zip";
            return new ZipFilePacker(zipFilePath);
        }

        public int Run()
        {

            var connectionId = _connectionIdArg.Value;
            var storage = new ConnectionStorage(_options.ConfigFilePath);
            var info = storage.Get(connectionId);
            if (info == null) {
                Console.WriteLine("Connection with current ID is not found: " + connectionId);
                return -1;
            }

            InitConnection(info);

            var exporter = new DbExporter(GetDbReader(), GetDatasetExporter(), GetPacker());

            Console.WriteLine($"Exporting database...");
            exporter.Export();
            Console.WriteLine($"Exporting completed");

            return 0;
        }
    }
}
