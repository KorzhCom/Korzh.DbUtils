using System;
using System.IO;

namespace Korzh.DbUtils.Export
{
    public class DbExporter
    {
        private readonly IDbBridge _dbBridge;
        private readonly IDatasetExporter _datasetExporter;
        private readonly IDataPacker _dataPacker;

        public DbExporter(IDbBridge dbBridge, IDatasetExporter dataSetExporter, IDataPacker packer)
        {
            _dbBridge = dbBridge;
            _datasetExporter = dataSetExporter;
            _dataPacker = packer;
        }

        public void Export()
        {
            var tables = _dbBridge.GetTableNames();
            if (tables.Count > 0) {
                _dataPacker.Start();
                try {
                    foreach (var tableName in tables) {
                        using (var stream = GetPackerStream(tableName))
                        using (var reader = _dbBridge.GetDataReaderForTable(tableName)) {
                            _datasetExporter.Export(reader, stream, tableName);
                        }
                    }
                }
                finally {
                    _dataPacker.Finish();
                }
            }
        }

        protected virtual Stream GetPackerStream(string datasetName)
        {
            return _dataPacker.OpenStream(datasetName + "." + _datasetExporter.FormatExtension);
        }
    }
}
