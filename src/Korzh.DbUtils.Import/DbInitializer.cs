using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Korzh.DbUtils.Import;
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Represents different options for DbInitializer class
    /// Implements the <see cref="Korzh.DbUtils.IDbUtilsOptions" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDbUtilsOptions" />
    public class DbInitializerOptions : IDbUtilsOptions
    {
        /// <summary>
        /// Gets or sets the folder where unpacker will look for the data.
        /// </summary>
        /// <value>The seed data folder.</value>
        public string SeedDataFolder { get; set; }

        /// <summary>
        /// Gets or sets the database writer.
        /// </summary>
        /// <value>The database writer.</value>
        public IDbWriter DbWriter { get; set; }

        /// <summary>
        /// Gets or sets the dataset importer.
        /// </summary>
        /// <value>The dataset importer.</value>
        public IDatasetImporter DatasetImporter { get; set; }

        /// <summary>
        /// Gets or sets the unpacker.
        /// </summary>
        /// <value>The unpacker.</value>
        public IDataUnpacker Unpacker { get; set; }

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbInitializerOptions"/> class.
        /// </summary>
        public DbInitializerOptions() {
            SeedDataFolder = System.IO.Path.Combine("App_Data", "SeedData");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbInitializerOptions"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public DbInitializerOptions(ILoggerFactory loggerFactory) 
            : this()
        {
            LoggerFactory = loggerFactory;
        }
    }


    /// <summary>
    /// Represents the database initializer. 
    /// An object of this class can seeds the DB with the data with the help of provided dataset importer and unpacker
    /// </summary>
    public class DbInitializer
    {
        private readonly DbImporter _dbImporter;

        private DbInitializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbInitializer"/> class.
        /// </summary>
        /// <param name="options">The options of DB initializer</param>
        public DbInitializer(DbInitializerOptions options)
        {
            _dbImporter = new DbImporter(options.DbWriter, options.DatasetImporter, options.Unpacker);
            _options = options;
        }

        /// <summary>
        /// Starts the data seeding operation.
        /// </summary>
        public void Seed()
        {
            _dbImporter.Import();
        }


        /// <summary>
        /// Creates and returns a DB initializer according to options defined in the <paramref name="initAction"/> function.
        /// </summary>
        /// <param name="initAction">The action where we can set different options of the create DbInitializer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>DbInitializer.</returns>
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
