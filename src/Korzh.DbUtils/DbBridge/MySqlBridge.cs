using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.DbBridges
{
    public class MySqlBride : IDbReader, IDbWriter
    {
        private MySqlConnection _connection = null;
        private readonly string _connectionString;

        public MySqlBride(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlBride(MySqlConnection connection)
        {
            _connection = connection;
            _connectionString = connection.ConnectionString;
        }

        private void CheckConnection()
        {
            if (_connection == null) {
                _connection = new MySqlConnection(_connectionString);
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
            var command = _connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }

        public IDataReader GetDataReaderForTable(string tableName)
        {
            return GetDataReaderForSql("SELECT * FROM `" + tableName + "`");
        }

        public IReadOnlyCollection<DatasetInfo> GetDatasets()
        {
            var tables = new List<DatasetInfo>();
            using (var dataReader = GetDataReaderForSql("SHOW TABLES")) {
                while (dataReader.Read()) {
                    string tableName = dataReader.GetString(0);
                    tables.Add(new DatasetInfo(tableName));
                }
            }
            return tables.AsReadOnly();
        }

        public void WriteRecord(string tableName, IDataRecord record)
        {
            var sqlBuilder = new StringBuilder(100);
            sqlBuilder.AppendFormat("INSERT INTO `0` (", tableName);

            for (var i = 0; i < record.FieldCount; i++) {
                sqlBuilder.AppendFormat("`{0}`, ",  record.GetName(i));
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
            sqlBuilder.Append(") VALUES (");

            for (var i = 0; i < record.FieldCount; i++) {
           //     sqlBuilder.AppendFormat("`{0}`, ", );
            }

        }
    }
}
