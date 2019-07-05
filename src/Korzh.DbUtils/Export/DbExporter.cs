using System;
using System.IO;

namespace Korzh.DbUtils.Export
{
    public class DbExporter
    {
        private readonly IDbBridge _dbBridge;
        private readonly IDatasetExporter _datasetExporter;

        public DbExporter(IDbBridge dbBridge, IDatasetExporter dataSetExporter)
        {
            _dbBridge = dbBridge;
            _datasetExporter = dataSetExporter;
        }

        public void Export()
        {
            var tables = _dbBridge.GetTableNames();
            if (tables.Count > 0) {
                foreach (var tableName in tables) {
                    using (var stream = GetPackerStream(tableName))
                    using (var reader = _dbBridge.GetDataReaderForTable(tableName)) {
                        _datasetExporter.Export(reader, stream, tableName);
                    }
                }
            }
        }

        protected virtual Stream GetPackerStream(string datasetName)
        {
            return new System.IO.FileStream(datasetName + ".xml", System.IO.FileMode.Create);
        }
    }
}
