using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Korzh.DbUtils.Import;
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{

    public class DbInitializerOptions : IDbUtilsOptions
    {
        public string SeedDataFolder { get; set; } 
        public IDbSeeder DbWriter { get; set; }
        public IDatasetImporter DatasetImporter { get; set; }
        public IDataUnpacker Unpacker { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public DbInitializerOptions() {
            SeedDataFolder = System.IO.Path.Combine("App_Data", "SeedData");
        }

        public DbInitializerOptions(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }
    }
    public class DbInitializer
    {
        private readonly DbImporter _dbImporter;

        private DbInitializerOptions _options;

        public DbInitializer(DbInitializerOptions options)
        {
            _dbImporter = new DbImporter(options.DbWriter, options.DatasetImporter, options.Unpacker);
            _options = options;
        }

        public void Seed()
        {
            _dbImporter.Import();
        }


        public static DbInitializer Create(Action<IDbUtilsOptions> initAction, ILoggerFactory loggerFactory = null)
        {
            var options = new DbInitializerOptions(loggerFactory);

            initAction?.Invoke(options);

            if (options.DatasetImporter == null) {
                options.DatasetImporter = new JsonDatasetImporter(options.LoggerFactory);
            }

            if (options.Unpacker == null) {
                options.Unpacker = new FileFolderPacker(options.SeedDataFolder);
            }

            return new DbInitializer(options);
        }
    }

}
