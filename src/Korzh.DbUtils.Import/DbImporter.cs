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
                    using (var datasetStream = _dataUnpacker.OpenStreamForUnpacking(table.Name)) {
                        if (datasetStream != null) {
                            _logger?.LogInformation($"Importing {table.Name}...");
                            int errorCount = 0;
                            int recordCount = 0;
                            var dataset = _datasetImporter.StartImport(datasetStream);
                            dataset.SetSchema(table.Schema); //!!!!! TODO: need to save schema with exported file and then read it from there
                            _dbWriter.StartSeeding(dataset);
                            try {
                                while (_datasetImporter.HasRecords()) {
                                    try {
                                        _dbWriter.InsertRecord(_datasetImporter.NextRecord());
                                        recordCount++;
                                    }
                                    catch (Exception ex) {
                                        _logger?.LogDebug("ERROR: " + ex.Message);
                                        errorCount++;
                                    }
                                }
                            }
                            finally {
                                _dbWriter.FinishSeeding();
                                _datasetImporter.FinishImport();

                                _logger?.LogInformation($"{recordCount} records were imported");

                                if (errorCount > 0) {
                                    _logger?.LogWarning($"{errorCount} errors during import (duplicate records or violated constraints)");
                                }
                            }
                        }
                    }
                }
            }
            finally {
                _dataUnpacker.FinishUnpacking();
            }    
        }
    }
}
