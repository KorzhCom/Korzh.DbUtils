using System;
using System.IO;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using McMaster.Extensions.CommandLineUtils;

namespace Korzh.DbTool
{
    class Program
    {

        public static ILoggerFactory LoggerFactory;

        static Program()
        {
            LoggerFactory = GetLoggerFactory();
        }

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            RootCommand.Configure(app);
            return app.Execute(args);
        }

        private static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                   builder.AddConsole());
            return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        }
    }

    // Commands/RootCommand.cs
    public class RootCommand : ICommand
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Name = "dbtool";
            app.HelpOption("-?|-h|--help");

            var options = new GlobalOptions();

            options.LocalConfigFilePathOption = 
                            app.Option("--config|-c", 
                                       "Config file path", 
                                        CommandOptionType.SingleValue)
                               .Accepts(configure => configure.ExistingFile("Configuration file does not exist"));

            options.FormatOption = app.Option("--format|-f",
                                              "Exporting/importing format (xml | json)",
                                              CommandOptionType.SingleValue)
                                      .Accepts(config => config.Values(true, "xml", "json"));


            // Register commands
            app.Command("export", c => ExportCommand.Configure(c, options));
            app.Command("import", c => ImportCommand.Configure(c, options));
            app.Command("connections", c => ConnectionsCommand.Configure(c, options));

            Func<int> runCommandFunc = new RootCommand(app, options).Run;
            app.OnExecute(runCommandFunc);
        }

        private readonly CommandLineApplication _app;
        private readonly GlobalOptions _options;

        public RootCommand(CommandLineApplication app, GlobalOptions options)
        {
            _app = app;
            _options = options;
        }

        public int Run()
        {
            _app.ShowHelp();

            Console.WriteLine("Current options:");
            Console.WriteLine("Config path:" + _options.LocalConfigFilePathOption.Value());
            Console.WriteLine("Format:" + _options.Format);

            return 0;
        }
    }
}
