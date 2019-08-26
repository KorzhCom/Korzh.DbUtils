using System;
using System.IO;
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

        public class ArgumentsAndOptions
        {

            public readonly CommandArgument ConnectionIdArg;

            public readonly CommandOption OutputPathOption;

            public readonly CommandOption ZipOption;

            public ArgumentsAndOptions(CommandLineApplication command)
            {
                ConnectionIdArg = command.Argument("<connection ID>", "The ID of some previously registered connection")
                                           .IsRequired();

                OutputPathOption = command.Option("--output|-o:<OUTPUT_DIRECTORY>", "Directory in which to place the export files", CommandOptionType.SingleOrNoValue)
                                           .Accepts(config => config.LegalFilePath());

                ZipOption = command.Option("--zip:<ZIP_NAME>", "Pack result to zip package", CommandOptionType.SingleOrNoValue);
            }

            public string ConnectionId => ConnectionIdArg.Value;

            public string OutputPath => OutputPathOption.HasValue() ? OutputPathOption.Value() : ".";

            public string PackToZip => ZipOption.HasValue() ? ZipOption.Value() : null;

        }

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Exports some DB to a backup archive in the specified format (XML, JSON)";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.FormatOption);
            command.Options.Add(options.LocalConfigFilePathOption);

            var arguments = new ArgumentsAndOptions(command);

            Func<int> runCommandFunc = new ExportCommand(arguments, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly ArgumentsAndOptions _arguments;
        private readonly GlobalOptions _options;

        private DbConnection _connection;

        public ExportCommand(ArgumentsAndOptions arguments, GlobalOptions options)
        {
            _options = options;
            _arguments = arguments;
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
                Console.WriteLine($"Openning {info.Id} connection...");
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
                return new SqlServerBridge(_connection as SqlConnection, Program.LoggerFactory);
            }
            else if (_connection is MySqlConnection){
                return new MySqlBridge(_connection as MySqlConnection, Program.LoggerFactory);
            }

            return null;

        }

        private IDataPacker GetPacker()
        {
            if (_arguments.ZipOption.HasValue()) {

                if (_arguments.OutputPathOption.HasValue()) {
                    Directory.CreateDirectory(_arguments.OutputPath);
                }

                string zipFileName = _arguments.PackToZip;
                if (zipFileName != null && !zipFileName.EndsWith(".zip")) {
                    zipFileName += ".zip";
                }

                var zipFilePath = Path.Combine(_arguments.OutputPath, zipFileName ?? _arguments.ConnectionId + "_" 
                                              + DateTime.Now.ToString("yyyy-MM-dd") + ".zip");

                return new ZipFilePacker(zipFilePath, Program.LoggerFactory);
            }
            else if (_arguments.OutputPathOption.HasValue()) {
                Directory.CreateDirectory(_arguments.OutputPath);
                return new FileFolderPacker(_arguments.OutputPath, Program.LoggerFactory);
            }

            var directory = _arguments.ConnectionId + "_" + DateTime.Now.ToString("yyyy-MM-dd");

            Directory.CreateDirectory(directory);

            return new FileFolderPacker(directory, Program.LoggerFactory);
        }

        public int Run()
        {
            var storage = new ConnectionStorage(_options.ConfigFilePath);
            var info = storage.Get(_arguments.ConnectionId);
            if (info == null) {
                Console.WriteLine("Connection with current ID is not found: " + _arguments.ConnectionId);
                return -1;
            }

            InitConnection(info);

            var exporter = new DbExporter(GetDbReader(), GetDatasetExporter(), GetPacker());

            Console.WriteLine($"Exporting database [{_arguments.ConnectionId}]...");
            exporter.Export();
            Console.WriteLine($"Export completed!");

            return 0;
        }
    }
}
