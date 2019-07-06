using System;

using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils.Import
{

    public class DbImporter
    {
        private readonly IDbBridge _dbBridge;
        private readonly IDatasetImporter _datasetImporter;
        private readonly IDataUnpacker _dataUnpacker;

        public DbImporter(IDbBridge dbBridge, IDatasetImporter datasetImporter, IDataUnpacker unpacker)
        {
            _dbBridge = dbBridge;
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
                            _dbBridge.WriteRecord(_datasetImporter.NextRecord());
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
