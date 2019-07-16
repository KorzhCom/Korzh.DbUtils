using System;

using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils.Import
{
    /// <summary>
    /// Imports the content of some DB stored in some commmand format (JSON, XML, etc)
    /// </summary>
    public class DbImporter
    {
        private readonly IDbSeeder _dbWriter;
        private readonly IDatasetImporter _datasetImporter;
        private readonly IDataUnpacker _dataUnpacker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbImporter"/> class.
        /// </summary>
        /// <param name="dbWriter">The database writer - an object which implements <see cref="IDbSeeder"/> interface.</param>
        /// <param name="datasetImporter">The dataset importer - knows how to read one dataset data stored in a particular format.</param>
        /// <param name="unpacker">The unpacker - knows how to find the data for a particular dataset "packed" in "archive".</param>
        public DbImporter(IDbSeeder dbWriter, IDatasetImporter datasetImporter, IDataUnpacker unpacker)
        {
            _dbWriter = dbWriter;
            _datasetImporter = datasetImporter;
            _dataUnpacker = unpacker;
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
                    _dbWriter.StartSeeding(table);
                    try {
                        using (var datasetStream = _dataUnpacker.OpenStreamForUnpacking(table.Name)) {
                            if (datasetStream != null) {
                                var dataset = _datasetImporter.StartImport(datasetStream);
                                while (_datasetImporter.HasRecords()) {
                                    _dbWriter.WriteRecord(_datasetImporter.NextRecord());
                                }
                                _datasetImporter.FinishImport();
                            }
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message + ":" + ex.StackTrace); //remove in future or make with logging
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
