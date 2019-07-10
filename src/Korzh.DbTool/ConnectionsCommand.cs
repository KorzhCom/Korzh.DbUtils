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
            command.Command("add", c => AddConnectionCommand.Configure(c, options));
            command.Command("remove", c => RemoveConnectionCommand.Configure(c, options));
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

    public class AddConnectionCommand : ICommand
    {

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Adds connection to configuration (add, remove, list)";
            command.HelpOption("-?|-h|--help");

            command.Options.Add(options.LocalConfigFilePathOption);

            command.Option("--dbtype", "Database type: mssql, mysql", CommandOptionType.SingleValue);
            //command.Option("--cs");


        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }

    public class RemoveConnectionCommand : ICommand
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

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }



}
