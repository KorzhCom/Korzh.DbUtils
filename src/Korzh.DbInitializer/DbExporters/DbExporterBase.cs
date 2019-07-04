using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Korzh.DbUtils.DbSavers;

/// <summary>
/// The DbExporters namespace.
/// </summary>
namespace Korzh.DbUtils.Exporters
{
    public abstract class DbExporterBase : IDbExporter
    {

        private readonly string _connectionString;
        private readonly IDbSaver _saver;

        protected DbConnection DbConnection;

        public DbExporterBase(string connectionString, IDbSaver saver)
        {
            _connectionString = connectionString;
            _saver = saver;
        }

        public void Export()
        {
            DbConnection.Open();

            var tables = GetTables();
            if (tables.Count > 0) {

                _saver.Start();
                foreach (var table in tables)
                {
                    _saver.StartSaveTable(table);
                    using (var reader = ReadTable(table))
                    {
                        _saver.SaveTableData(reader);
                    }
                    _saver.EndSaveTable();
                }
                _saver.End();
            }

            DbConnection.Close();
        }

        protected abstract IReadOnlyCollection<string> GetTables();

        protected abstract IDataReader ReadTable(string tableName);
    }
}
