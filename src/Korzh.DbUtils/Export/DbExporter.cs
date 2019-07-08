using System;
using System.IO;

namespace Korzh.DbUtils.Export
{
    public class DbExporter
    {
        private readonly IDbReader _dbReader;
        private readonly IDatasetExporter _datasetExporter;
        private readonly IDataPacker _dataPacker;

        public DbExporter(IDbReader dbReader, IDatasetExporter dataSetExporter, IDataPacker packer)
        {
            _dbReader = dbReader;
            _datasetExporter = dataSetExporter;
            _dataPacker = packer;
        }

        public void Export()
        {
            var tables = _dbReader.GetTableNames();
            if (tables.Count > 0) {
                _dataPacker.StartPacking();
                try {
                    foreach (var tableName in tables) {
                        using (var stream = GetPackerStream(tableName))
                        using (var reader = _dbReader.GetDataReaderForTable(tableName)) {
                            _datasetExporter.ExportDataset(reader, stream, tableName);
                        }
                    }
                }
                finally {
                    _dataPacker.FinishPacking();
                }
            }
        }

        protected virtual Stream GetPackerStream(string datasetName)
        {
            return _dataPacker.OpenStreamForPacking(datasetName + "." + _datasetExporter.FormatExtension);
        }
    }
}
