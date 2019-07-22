using System;
using System.IO;

namespace Korzh.DbUtils.Export
{
    /// <summary>
    /// Allows you to export the content of some database to a bunch of files of particular format (XML, JSON, etc)
    /// </summary>
    public class DbExporter
    {
        private readonly IDbReader _dbReader;
        private readonly IDatasetExporter _datasetExporter;
        private readonly IDataPacker _dataPacker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExporter"/> class.
        /// </summary>
        /// <param name="dbReader">A database reader - an object which implements <see cref="IDbReader"/> interface 
        /// and provide some basic operation for reading the DB content.</param>
        /// <param name="dataSetExporter">A dataset exporter - allows to save the content of some dataset in a file of a particular format.</param>
        /// <param name="packer">A packer - packs the files created by dataset exporter to some storage (e.g. to a folder or a ZIP archive).</param>
        public DbExporter(IDbReader dbReader, IDatasetExporter dataSetExporter, IDataPacker packer)
        {
            _dbReader = dbReader;
            _datasetExporter = dataSetExporter;
            _dataPacker = packer;
        }

        /// <summary>
        /// Performs the exporting.
        /// This method navigates through all tables in a database, 
        /// exports the content of each of them to the specified data format and then 
        /// packs the result files with the specified packer.
        /// </summary>
        public void Export()
        {
            var datasets = _dbReader.GetDatasets();
            if (datasets.Count > 0) {
                _dataPacker.StartPacking(_datasetExporter.FileExtension);
                try {
                    foreach (var table in datasets) {

                        if (string.Equals(table.Name, "__EFMigrationsHistory", StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        using (var stream = GetPackerStream(table.Name))
                        using (var reader = _dbReader.GetDataReaderForTable(table)) {
                            _datasetExporter.ExportDataset(reader, stream, table);
                        }
                    }
                }
                finally {
                    _dataPacker.FinishPacking();
                }
            }
        }

        /// <summary>
        /// Opens the write stream for packing one dataset (table).
        /// </summary>
        /// <param name="datasetName">The name of the dataset.</param>
        /// <returns>Stream.</returns>
        protected virtual Stream GetPackerStream(string datasetName)
        {
            return _dataPacker.OpenStreamForPacking(datasetName);
        }
    }
}
