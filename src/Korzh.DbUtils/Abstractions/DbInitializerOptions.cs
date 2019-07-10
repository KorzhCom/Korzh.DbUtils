using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils
{
    public class DbInitializerOptions
    {
        public string InitialDataFolder { get; set; }

        public IDbWriter DbWriter { get; set; }

        public IDatasetImporter DatasetImporter { get; set; }

        public IDataUnpacker Unpacker { get; set; }

        public bool NeedDataSeeding { get; set; } = false;

        public ILoggerFactory LoggerFactory { get; private set; }

        public DbInitializerOptions()
        {
            InitialDataFolder = System.IO.Path.Combine("App_Data", "InitialData");
        }

        public DbInitializerOptions(ILoggerFactory loggerFactory) : this()
        {
            LoggerFactory = loggerFactory;
        }
    }
}
