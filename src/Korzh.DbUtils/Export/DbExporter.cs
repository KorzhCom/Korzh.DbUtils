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
            var datasets = _dbReader.GetDatasets();
            if (datasets.Count > 0) {
                _dataPacker.StartPacking();
                try {
                    foreach (var table in datasets) {
                        using (var stream = GetPackerStream(table.Name))
                        using (var reader = _dbReader.GetDataReaderForTable(table.Name)) {
                            _datasetExporter.ExportDataset(reader, stream, table.Name);
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
