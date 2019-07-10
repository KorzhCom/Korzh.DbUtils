
using McMaster.Extensions.CommandLineUtils;

namespace Korzh.DbTool
{
    public class GlobalOptions
    {
        internal CommandOption LocalConfigFilePathOption { get; set; }

        internal CommandOption FormatOption { get; set; }

        public string Format {
            get {
                return FormatOption.HasValue() ? FormatOption.Value().ToLower() : "json";
            }
        }

    }
}
