using System;
using System.Collections.Generic;
using System.Text;

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

            command.OnExecute(new ConnectionsCommand(command).Run);
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

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Adds connection to configuration (add, remove, list)";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.LocalConfigFilePathOption);

            var connectionIdArg = command.Argument("Connection ID", "The connection ID stored in the configuration");
            var dbTypeArg = command.Argument("Database type", "The database type (mssql, mysql)");
            var connectionStringArg = command.Argument("Connection string", "The connection string to add");

            command.OnExecute(new ConnectionsAddCommand(connectionIdArg, dbTypeArg, connectionStringArg, options.LocalConfigFilePathOption).Run);
        }

        private readonly CommandArgument _connectionIdArg;
        private readonly CommandArgument _dbTypeArg;
        private readonly CommandArgument _connectionStringArg;

        private readonly CommandOption _localConfigFilePathOption;

        public ConnectionsAddCommand(
            CommandArgument connectionIdArg, 
            CommandArgument dbTypeArg, 
            CommandArgument connectionStringArg,
            CommandOption localConfigFilePathOption)
        {
            _connectionIdArg = connectionIdArg;
            _dbTypeArg = dbTypeArg;
            _connectionStringArg = connectionStringArg;

            _localConfigFilePathOption = localConfigFilePathOption;
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectionsRemoveCommand : ICommand
    {
        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectionsListCommand : ICommand
    {
        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {

            command.Description = "Shows list of the connections stored in the configuration file.";

            command.Options.Add(options.LocalConfigFilePathOption);

            command.OnExecute(new ConnectionsListCommand(options.LocalConfigFilePathOption).Run);
        }

        private readonly CommandOption _localConfigFilePathOption;

        public ConnectionsListCommand(CommandOption localConfigFilePathOption)
        {
            _localConfigFilePathOption = localConfigFilePathOption;
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }



}
