using System;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

using MySql.Data.MySqlClient;

using McMaster.Extensions.CommandLineUtils;

using Korzh.DbUtils;
using Korzh.DbUtils.Import;
using Korzh.DbUtils.SqlServer;
using Korzh.DbUtils.MySql;
using Korzh.DbUtils.Postgre;
using Korzh.DbUtils.Packing;
using System.Threading;
using Npgsql;


namespace Korzh.DbTool
{
    public class ImportCommand : ICommand
    {

        public class ArgumentsAndOptions
        {
            public readonly CommandArgument ConnectionIdArg;

            public readonly CommandOption InputPathOption;

            public readonly CommandOption FormatOption;

            public ArgumentsAndOptions(CommandLineApplication command)
            {
                ConnectionIdArg = command.Argument("<connection ID>", "The ID of some previously registered connection")
                                           .IsRequired();

                InputPathOption = command.Option("--input|-i:<INPUT_DIRECTORY(INPUT_ZIP)>", "Directory or zip file in which contains the import files", CommandOptionType.SingleValue)
                                           .Accepts(config => config
                                                           .LegalFilePath()
                                                           .ExistingFileOrDirectory())
                                           .IsRequired();
            }

            public string ConnectionId => ConnectionIdArg.Value;

            public string InputPath => InputPathOption.Value();

        }


        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Imports some DB from a backup archive with the specified format (XML, JSON)";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.FormatOption);
            command.Options.Add(options.LocalConfigFilePathOption);

            var arguments = new ArgumentsAndOptions(command);

            Func<int> runCommandFunc = new ImportCommand(arguments, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly ArgumentsAndOptions _arguments;
        private readonly GlobalOptions _options;

        private DbConnection _connection;

        public ImportCommand(ArgumentsAndOptions arguments, GlobalOptions options)
        {
            _arguments = arguments;
            _options = options;
        }

        private void InitConnection(ConnectionInfo info)
        {
            switch (info.DbType)
            {
                case DbType.SqlServer:
                    _connection = new SqlConnection(info.ConnectionString);
                    break;
                case DbType.MySql:
                    _connection = new MySqlConnection(info.ConnectionString);
                    break;
                case DbType.PostgreSql:
                    _connection = new NpgsqlConnection(info.ConnectionString);
                    break;
                default:
                    throw new Exception("Unknown connection type: " + info.DbType);
            }

            if (_connection.State != ConnectionState.Open) {
                Console.WriteLine($"Openning {info.Id} connection...");
                _connection.Open();
            }
        }

        private IDatasetImporter GetDatasetImporter()
        {
            string format = _options.Format;
            if (!_options.FormatOption.HasValue()) {
                FileAttributes attr = File.GetAttributes(_arguments.InputPath);
                format = (attr & FileAttributes.Directory) == FileAttributes.Directory
                       ? DirectoryFormat(_arguments.InputPath)
                       : ZipArchiveFormat(_arguments.InputPath);
            }

            switch (format)
            {
                case "xml":
                    return new XmlDatasetImporter();
                default:
                    return new JsonDatasetImporter();
            }
        }

        private IDbWriter GetDbSeeder()
        {
            if (_connection is SqlConnection) {
                return new SqlServerBridge(_connection as SqlConnection, Program.LoggerFactory);
            }
            else if (_connection is MySqlConnection) {
                return new MySqlBridge(_connection as MySqlConnection, Program.LoggerFactory);
            }
            else if (_connection is NpgsqlConnection) {
                return new PostgreBridge(_connection as MySqlConnection, Program.LoggerFactory);
            }

            return null;

        }

        private IDataUnpacker GetUnpacker()
        {
            FileAttributes attr = File.GetAttributes(_arguments.InputPath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {

                return new FileFolderPacker(_arguments.InputPath, Program.LoggerFactory);
            }

            if (IsValidZipFile(_arguments.InputPath))
                return new ZipFilePacker(_arguments.InputPath, Program.LoggerFactory);

            throw new Exception("Zip is not valid.");
        }

        private bool IsValidZipFile(string path)
        {
            try {
                using (var zipFile = ZipFile.OpenRead(path)) {
                    var entries = zipFile.Entries;
                    return true;
                }
            }
            catch (InvalidDataException) {
                return false;
            }
        }

        private string DirectoryFormat(string path)
        {
            var files = Directory.GetFiles(path);

            if (files.All(f => f.EndsWith(".json"))) {
                return "json";
            }
            else if (files.All(f => f.EndsWith(".xml"))) {
                return "xml";
            }

            throw new Exception("Unknown format of imported files.  Use --format option to specify the format.");
        }

        private string ZipArchiveFormat(string path)
        {
            using (var zipFile = ZipFile.OpenRead(path)) {
                var entries = zipFile.Entries;
                if (entries.All(e => e.FullName.EndsWith(".json"))) {
                    return "json";
                }
                else if (entries.All(e => e.FullName.EndsWith(".xml"))) {
                    return "xml";
                }

                throw new Exception("Unknown format of imported files in the ZIP. Use --format option to specify the format.");
            }
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

            var dbSeeder = GetDbSeeder();
            var unpacker = GetUnpacker();
            var dsImported = GetDatasetImporter();

            Console.WriteLine($"Importing data to [{_arguments.ConnectionId}]...");
            new DbImporter(dbSeeder, dsImported, unpacker, Program.LoggerFactory).Import();
            Thread.Sleep(100); //to finish logger rendering
            Console.WriteLine("Import completed!");

            return 0;
        }
    }
}
