using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils
{
    public interface IDbUtilsOptions
    {
        string SeedDataFolder { get; set; }

        IDbWriter DbWriter { get; set; }

        IDatasetImporter DatasetImporter { get; set; }

        IDataUnpacker Unpacker { get; set; }

        ILoggerFactory LoggerFactory { get; set; }

    }
}
