using System;
using System.Collections.Generic;
using System.Text;

using McMaster.Extensions.CommandLineUtils;

namespace Korzh.DbTool
{
    public class FilterTablesCommand : ICommand
    {

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Updates the connection filter: sets the list of tables which will be processed on export. ";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.LocalConfigFilePathOption);

            var connectionArg = command.Argument("<сonnection ID>", "The ID of some previously registered connection")
                                  .IsRequired();

            var tablesArg = command.Argument("<tables>", "Table names separeated by comma. Leave this argument empty to clear the connection filter.");

            Func<int> runCommandFunc = new FilterTablesCommand(connectionArg, tablesArg, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly GlobalOptions _options;
        private readonly CommandArgument _connectionIdArg;
        private readonly CommandArgument _tablesArg;

        public FilterTablesCommand(CommandArgument connectionIdArg, CommandArgument tablesArg, GlobalOptions globalOptions)
        {
            _tablesArg = tablesArg;
            _options = globalOptions;
            _connectionIdArg = connectionIdArg;
        }

        public int Run()
        {
            var connectionId = _connectionIdArg.Value;
            var tables = _tablesArg.Value;
            var storage = new ConnectionStorage(_options.ConfigFilePath);
            var connection = storage.Get(connectionId);
            if (connection is null) {
                Console.WriteLine($"Connection not found: {connectionId}");
                return -1;
            }

            connection.Tables = tables;
            storage.AddUpdate(connectionId, connection);

            storage.SaveChanges();

            if (!string.IsNullOrEmpty(tables)) {
                Console.WriteLine($"The connection [{connectionId}] is now filtered by tables: " + tables);
            }
            else {
                Console.WriteLine($"The connection's ({connectionId}) filter has been cleared");
            }

            return 0;
        }
    }
}
