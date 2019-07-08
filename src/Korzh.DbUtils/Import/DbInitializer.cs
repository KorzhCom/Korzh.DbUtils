using System;

using Korzh.DbUtils.Import;
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{
    public enum DbBackupFormat {
        JSON,

        XML
    }


    public class DbInitializer
    {
        private readonly DbImporter _dbImporter;

        public DbInitializer(DbInitializerOptions options)
        {
            _dbImporter = new DbImporter(options.DbWriter, options.DatasetImporter, options.Unpacker);
        }

        public void Run()
        {
            _dbImporter.Import();
        }


        public static DbInitializer Create(Action<DbInitializerOptions> initAction)
        {
            var options = new DbInitializerOptions();

            initAction?.Invoke(options);

            if (options.DatasetImporter == null) {
                options.DatasetImporter = new JsonDatasetImporter();
            }

            if (options.Unpacker == null) {
                options.Unpacker = new FileFolderPacker(options.InitialDataFolder);
            }

            return new DbInitializer(options);
        }
    }

    public class DbInitializerOptions
    {
        public string InitialDataFolder { get; set; }

        public IDbWriter DbWriter { get; set; }

        public IDatasetImporter DatasetImporter { get; set; }

        public IDataUnpacker Unpacker { get; set; }

        public DbInitializerOptions() {
            InitialDataFolder = System.IO.Path.Combine("App_Data", "InitialData");
        }
    }
}
