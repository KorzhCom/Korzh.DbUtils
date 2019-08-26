using System;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Import
{
    /// <summary>
    /// Imports the content of some DB stored in some commmand format (JSON, XML, etc)
    /// </summary>
    public class DbImporter
    {
        private readonly IDbWriter _dbWriter;
        private readonly IDatasetImporter _datasetImporter;
        private readonly IDataUnpacker _dataUnpacker;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbImporter"/> class.
        /// </summary>
        /// <param name="dbWriter">The database writer - an object which implements <see cref="IDbWriter"/> interface.</param>
        /// <param name="datasetImporter">The dataset importer - knows how to read one dataset data stored in a particular format.</param>
        /// <param name="unpacker">The unpacker - knows how to find the data for a particular dataset "packed" in "archive".</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public DbImporter(IDbWriter dbWriter, IDatasetImporter datasetImporter, IDataUnpacker unpacker, ILoggerFactory loggerFactory = null)
        {
            _dbWriter = dbWriter;
            _datasetImporter = datasetImporter;
            _dataUnpacker = unpacker;
            _logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Starts the importing operation.
        /// </summary>
        public void Import()
        {
            _dataUnpacker.StartUnpacking(_datasetImporter.FileExtension);
            var datasets = _dbWriter.GetDatasets();
            try {
                foreach (var table in datasets) {
                    try {
                        using (var datasetStream = _dataUnpacker.OpenStreamForUnpacking(table.Name)) {
                            if (datasetStream != null) {
                                _logger?.LogInformation($"Importing {table.Name}...");
                                var dataset = _datasetImporter.StartImport(datasetStream);
                                _dbWriter.StartSeeding(dataset);

                                while (_datasetImporter.HasRecords()) {
                                    try {
                                        _dbWriter.WriteRecord(_datasetImporter.NextRecord());
                                    }
                                    catch (Exception ex) {
                                        _logger?.LogError(ex.Message);
                                    }
                                }
                                _datasetImporter.FinishImport();
                            }
                        }
                    }
                    finally {
                        _dbWriter.FinishSeeding();
                    }
                }
            }
            finally {
                _dataUnpacker.FinishUnpacking();
            }    
        }
    }
}
