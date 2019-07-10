using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.DbTool
{
    public class ConnectionsCommand : ICommand
    {

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Manipulates with connections";
            command.HelpOption("-?|-h|--help");

            var addSubcommand = command.Command("add", c=> AddConnectionCommand.Configure(c, options));

           
        }

        public ConnectionsCommand()
        {

        }


        public int Run()
        {
            throw new NotImplementedException();
        }
    }

    public class AddConnectionCommand : ICommand
    {

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteConnectionCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }

    public class ListConncetionsCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }



}
