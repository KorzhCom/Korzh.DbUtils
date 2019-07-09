using System;

using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils.Import
{

    public class DbImporter
    {
        private readonly IDbWriter _dbWriter;
        private readonly IDatasetImporter _datasetImporter;
        private readonly IDataUnpacker _dataUnpacker;

        public DbImporter(IDbWriter dbWriter, IDatasetImporter datasetImporter, IDataUnpacker unpacker)
        {
            _dbWriter = dbWriter;
            _datasetImporter = datasetImporter;
            _dataUnpacker = unpacker;
        }

        public void Import()
        {
            _dataUnpacker.StartUnpacking(_datasetImporter.FileExtension);
            var datasets = _dbWriter.GetDatasets();
            foreach (var table in datasets) {
                _dbWriter.StartSeeding(table);
                try {
                    using (var datasetStream = _dataUnpacker.OpenStreamForUnpacking(table.Name)) {
                        if (datasetStream != null) {
                            var dataset = _datasetImporter.StartImport(datasetStream);
                            while (_datasetImporter.HasRecords()) {
                                _dbWriter.WriteRecord(table, _datasetImporter.NextRecord());
                            }
                            _datasetImporter.FinishImport();
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message + ":" + ex.StackTrace); //remove in future or make with logging
                }
                finally {
                    _dbWriter.FinishSeeding(table);
                    _dataUnpacker.FinishUnpacking();
                }

            }
        }
    }

}
