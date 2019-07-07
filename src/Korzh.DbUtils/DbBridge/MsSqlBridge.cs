using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace Korzh.DbUtils.DbBridges
{
    public class MsSqlBridge : IDbBridge
    {

        private SqlConnection _connection = null;
        private string _connectionString;

        public MsSqlBridge(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MsSqlBridge(SqlConnection connection)
        {
            _connection = connection;
            _connectionString = connection.ConnectionString;
        }

        private void CheckConnection()
        {
            if (_connection == null) {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open) {
                _connection.Open();
            }
        }

        public IDbConnection GetConnection()
        {
            CheckConnection();
            return _connection;
        }

        public IDataReader GetDataReaderForSql(string sql)
        {
            var connection = GetConnection();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }

        public IDataReader GetDataReaderForTable(string tableName)
        {
            return GetDataReaderForSql("SELECT * FROM [" + tableName + "]");
        }

        public IReadOnlyCollection<string> GetTableNames()
        {
            CheckConnection();
            DataTable schemaTable = _connection.GetSchema(SqlClientMetaDataCollectionNames.Tables);
            var tables = new List<string>();

            foreach (DataRow row in schemaTable.Rows) {
                string tableType = (string)row["TABLE_TYPE"];
                string tableName = (string)row["TABLE_NAME"];
                if (tableType == "BASE TABLE")
                    tables.Add(tableName);
            }

            return tables.AsReadOnly();
        }

        public void WriteRecord(IDataRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
