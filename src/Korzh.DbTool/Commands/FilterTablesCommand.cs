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
            var exclueOption = command.Option("--exclude", "Exclude specified tables", CommandOptionType.NoValue);

            var connectionArg = command.Argument("<Connection ID>", "The ID of some previously registered connection")
                                  .IsRequired();

            var tablesArg = command.Argument("<tables>", "Table names separeated by comma. Leave this argument empty to clear the connection filter.");

            Func<int> runCommandFunc = new FilterTablesCommand(connectionArg, tablesArg, exclueOption, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly CommandArgument _connectionIdArg;
        private readonly CommandArgument _tablesArg;
        private readonly CommandOption _excludeOption;
        private readonly GlobalOptions _options;

        public FilterTablesCommand(CommandArgument connectionIdArg, CommandArgument tablesArg, 
                                    CommandOption excludeOption, GlobalOptions globalOptions)
        {
            _connectionIdArg = connectionIdArg;
            _tablesArg = tablesArg;
            _excludeOption = excludeOption;
            _options = globalOptions;
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

            var exclude = _excludeOption.Values.Count > 0;

            if (exclude) {
                connection.IncludeTables = tables;
            }
            else {
                connection.ExcludeTables = tables;
            }

            storage.AddUpdate(connectionId, connection);

            storage.SaveChanges();

            if (!string.IsNullOrEmpty(tables)) {
                if (exclude) {
                    Console.WriteLine($"Tables {tables} will be excluded for connection [{connectionId}]");
                }
                else {
                    Console.WriteLine($"Only tables {tables} will be processed for connection [{connectionId}]");
                }
            }
            else {
                Console.WriteLine($"The connection's ({connectionId}) filter has been cleared");
            }

            return 0;
        }
    }
}
