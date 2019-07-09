using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace Korzh.DbUtils.DbBridges
{
    public class MsSqlBridge : IDbReader, IDbWriter
    {

        private SqlConnection _connection = null;
        private readonly string _connectionString;

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

        public IReadOnlyCollection<DatasetInfo> GetDatasets()
        {
            CheckConnection();
            DataTable schemaTable = _connection.GetSchema(SqlClientMetaDataCollectionNames.Tables);
            var tables = new List<DatasetInfo>();

            foreach (DataRow row in schemaTable.Rows) {
                string tableType = (string)row["TABLE_TYPE"];
                string tableName = (string)row["TABLE_NAME"];
                if (tableType == "BASE TABLE") {
                    tables.Add(new DatasetInfo(tableName));
                }
            }

            return tables.AsReadOnly();
        }

        public void WriteRecord(string tableName, IDataRecord record)
        {
            WriteToConsole(record);
        }

        //!!!!!!!!!!!!!!!! Just for testing. Remove before release
        private void WriteToConsole(IDataRecord record)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < record.FieldCount; i++) {
                sb.Append(record.GetValue(i).ToString() + "; ");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
