using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Defines different options which can be used for initialization of other library objects
    /// </summary>
    public interface IDbUtilsOptions
    {
        /// <summary>
        /// Gets or sets the folder where unpacker will look for the data.
        /// </summary>
        /// <value>The seed data folder.</value>
        string SeedDataFolder { get; set; }

        /// <summary>
        /// Gets or sets the database writer.
        /// </summary>
        /// <value>The database writer.</value>
        IDbWriter DbWriter { get; set; }

        /// <summary>
        /// Gets or sets the dataset importer.
        /// </summary>
        /// <value>The dataset importer.</value>
        IDatasetImporter DatasetImporter { get; set; }

        /// <summary>
        /// Gets or sets the unpacker.
        /// </summary>
        /// <value>The unpacker.</value>
        IDataUnpacker Unpacker { get; set; }

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        ILoggerFactory LoggerFactory { get; set; }
    }
}
