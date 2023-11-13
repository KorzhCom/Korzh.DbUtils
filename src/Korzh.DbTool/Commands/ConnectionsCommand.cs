using System;
using System.Linq;

using McMaster.Extensions.CommandLineUtils;

namespace Korzh.DbTool
{

    public class ConnectionsCommand : ICommand
    {
        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Manipulates with connections (add, remove, list)";
            command.HelpOption("-?|-h|--help");

            // add local config option
            command.Options.Add(options.LocalConfigFilePathOption);

            // add connections subcommands
            command.Command("add", c => ConnectionsAddCommand.Configure(c, options));
            command.Command("remove", c => ConnectionsRemoveCommand.Configure(c, options));
            command.Command("list", c => ConnectionsListCommand.Configure(c, options));

            Func<int> runCommandFunc = new ConnectionsCommand(command).Run;
            command.OnExecute(runCommandFunc);
        }

        private CommandLineApplication _command;

        public ConnectionsCommand(CommandLineApplication command)
        {
            _command = command;
        }

        public int Run()
        {
            _command.ShowHelp();

            return 0;
        }
    }

    public class ConnectionsAddCommand : ICommand
    {

        public class Arguments {

            public readonly CommandArgument ConnectionIdArg;
            public readonly CommandArgument ConnectionStringArg;
            public readonly CommandArgument DbTypeArg;

            public Arguments(CommandLineApplication command)
            {
                ConnectionIdArg = command.Argument("<сonnection ID>", "The connection ID stored in the configuration")
                                         .IsRequired();

                DbTypeArg = command.Argument("<database type>", $"The database type ({string.Join(", ", DbTool.DbType.AllDbTypes)})")
                   .Accepts(config => config.Values(true, DbTool.DbType.AllDbTypes))
                   .IsRequired();

                ConnectionStringArg = command.Argument("<сonnection string>", "The connection string to add")
                                             .IsRequired();

            }

            public string ConnectionId => ConnectionIdArg.Value;
            public string ConnectionString => ConnectionStringArg.Value;
            public string DbType => DbTypeArg.Value;
        }

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Adds connection to configuration file";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.LocalConfigFilePathOption);

            var argumets = new Arguments(command);

            Func<int> runCommandFunc = new ConnectionsAddCommand(argumets, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly Arguments _arguments;

        private readonly GlobalOptions _options;

        public ConnectionsAddCommand(Arguments arguments, GlobalOptions globalOptions)
        {
            _arguments = arguments;
            _options = globalOptions;
        }

        public int Run()
        {
            var storage = new ConnectionStorage(_options.ConfigFilePath);

            storage.AddUpdate(_arguments.ConnectionId, 
                new ConnectionInfo(_arguments.ConnectionId, 
                        _arguments.DbType, 
                        _arguments.ConnectionString));

            storage.SaveChanges();

            Console.WriteLine($"Connection {_arguments.ConnectionId} has been added.");

            return 0;
        }
    }

    public class ConnectionsRemoveCommand : ICommand
    {
        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Removes connection from configuration file";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.LocalConfigFilePathOption);

            var connectionIdArg = command.Argument("<сonnection ID>", "The ID of the connection stored in the configuration")
                                        .IsRequired();

            Func<int> runCommandFunc = new ConnectionsRemoveCommand(connectionIdArg, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly CommandArgument _connectionIdArg;

        private readonly GlobalOptions _options;

        public ConnectionsRemoveCommand(CommandArgument connectionIdArg, GlobalOptions globalOptions)
        {
            _connectionIdArg = connectionIdArg;
            _options = globalOptions;
        }

        public int Run()
        {
            var storage = new ConnectionStorage(_options.ConfigFilePath);

            storage.Remove(_connectionIdArg.Value);
            storage.SaveChanges();

            Console.WriteLine($"Connection {_connectionIdArg.Value} has been removed.");

            return 0;
        }
    }

    public class ConnectionsListCommand : ICommand
    {
        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {

            command.Description = "Shows the list of connections.";

            command.Options.Add(options.LocalConfigFilePathOption);

            Func<int> runCommandFunc = new ConnectionsListCommand(options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly GlobalOptions _options;

        public ConnectionsListCommand(GlobalOptions globalOptions)
        {
            _options = globalOptions;
        }

        public int Run()
        {
            var storage = new ConnectionStorage(_options.ConfigFilePath);

            var connections = storage.List();
            if (connections.Any()) {
                var location = _options.LocalConfigFilePathOption.HasValue() ? _options.LocalConfigFilePathOption.Value() : "global";
                Console.WriteLine($"Connections ({location}): ");
                var num = 1;
                foreach (var connection in connections) {
                    var connDesc = $"{num:D2}: {connection.ConnectionId} [{connection.Info.DbType}]\n    \"{connection.Info.ConnectionString}\"";
                    if (!string.IsNullOrEmpty(connection.Info.IncludeTables)) {
                        connDesc += "\n    Include only: " + connection.Info.IncludeTables;
                    }
                    if (!string.IsNullOrEmpty(connection.Info.ExcludeTables)) {
                        connDesc += "\n    Exclude: " + connection.Info.ExcludeTables;
                    }
                    Console.WriteLine(connDesc);
                    Console.WriteLine();
                    num++;
                }
            }
            else {
                Console.WriteLine("No connections.");
            }

            return 0;
        }
    }



}
