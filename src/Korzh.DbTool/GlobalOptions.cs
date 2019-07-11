
using McMaster.Extensions.CommandLineUtils;

namespace Korzh.DbTool
{
    public class GlobalOptions
    {
        internal CommandOption LocalConfigFilePathOption { get; set; }

        internal CommandOption FormatOption { get; set; }

        public string Format => FormatOption.HasValue()
                             ? FormatOption.Value().ToLower() 
                             : "json";

        public string ConfigFilePath => LocalConfigFilePathOption.HasValue()
                                     ? LocalConfigFilePathOption.Value()
                                     : Settings.GlobalConfigFilePath;

    }
}
