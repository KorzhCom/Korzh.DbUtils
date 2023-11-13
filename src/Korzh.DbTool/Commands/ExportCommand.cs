using System;
using System.IO;
using System.Linq;
using System.Data.Common;
using System.Data;

using McMaster.Extensions.CommandLineUtils;

using Korzh.DbUtils;
using Korzh.DbUtils.Packing;
using Korzh.DbUtils.Export;

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

                OutputPathOption = command.Option("--output|-o=<OUTPUT_DIRECTORY>", "Directory in which to place the export files", CommandOptionType.SingleOrNoValue)
                                           .Accepts(config => config.LegalFilePath());

                ZipOption = command.Option("--zip=<ZIP_NAME>", "Pack result to zip package", CommandOptionType.SingleOrNoValue);
            }

            public string ConnectionId => ConnectionIdArg.Value;

            public string OutputPath => OutputPathOption.HasValue() ? OutputPathOption.Value() : ".";

            public string PackToZip => ZipOption.HasValue() ? ZipOption.Value() : null;

        }

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Exports specified DB to a backup archive in the specified format (XML, JSON)";
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
            _connection = ConnectionFactory.Create(info);

            if (_connection.State != ConnectionState.Open) {
                Console.WriteLine($"Opening {info.Id} connection...");
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
            return DbBridgeFactory.Create(_connection);
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

            var exporter = new DbExporter(GetDbReader(), GetDatasetExporter(), GetPacker(), Program.LoggerFactory);

            Console.WriteLine($"Exporting database [{_arguments.ConnectionId}]...");
            exporter.Export(info.GetDatasetFilter());
            Console.WriteLine($"Export completed!");

            return 0;
        }
    }
}
