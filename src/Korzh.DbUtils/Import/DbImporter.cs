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
            _dataUnpacker.StartUnpacking();
            try {
                while (_dataUnpacker.HasData()) {
                    using (var datasetStream = _dataUnpacker.NextDatasetStream()) {
                        _datasetImporter.StartImport(datasetStream);
                        while (_dataUnpacker.HasData()) {
                            _dbWriter.WriteRecord(_datasetImporter.NextRecord());
                        }
                        _datasetImporter.FinishImport();
                    }
                }
            }
            finally {
                _dataUnpacker.FinishUnpacking();
            }
        }
    }

}
