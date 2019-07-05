using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;


/// <summary>
/// Contains all classes which implement exporting operations.
/// </summary>
namespace Korzh.DbUtils.Export
{
    public abstract class DbExporterBase : IDbExporter
    {

        private readonly string _connectionString;
        private readonly IDatasetExporter _datasetExporter;

        protected DbConnection DbConnection;

        public DbExporterBase(string connectionString, IDatasetExporter saver)
        {
            _connectionString = connectionString;
            _datasetExporter = saver;
        }

        public void Export()
        {
            DbConnection.Open();

            var tables = GetTables();
            if (tables.Count > 0) {

                //_datasetExporter.Start();
                //foreach (var table in tables)
                //{
                //    _datasetExporter.StartSaveTable(table);
                //    using (var reader = ReadTable(table))
                //    {
                //        _datasetExporter.SaveTableData(reader);
                //    }
                //    _datasetExporter.EndSaveTable();
                //}
                //_datasetExporter.End();
            }

            DbConnection.Close();
        }

        protected abstract IReadOnlyCollection<string> GetTables();

        protected abstract IDataReader ReadTable(string tableName);
    }
}
