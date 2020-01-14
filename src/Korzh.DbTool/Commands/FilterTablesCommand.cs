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
            command.Description = "Adds table to use for the connection.";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.LocalConfigFilePathOption);

            var connectionArg = command.Argument("<сonnection ID>", "The connection ID stored in the configuration")
                                  .IsRequired();

            var tablesArg = command.Argument("<tables>", "Table names separeated by ,")
                              .IsRequired();

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
                Console.WriteLine($"Connection {connectionId} is not found.");
                return -1;
            }

            connection.Tables = tables;
            storage.Add(connectionId, connection);

            storage.SaveChanges();

            Console.WriteLine($"Connection {connectionId} has been updated with tables: " + tables);

            return 0;
        }
    }
}
