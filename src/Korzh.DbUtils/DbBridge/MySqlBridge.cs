using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.DbBridges
{
    public class MySqlBride : IDbBridge
    {
        private MySqlConnection _connection = null;
        private string _connectionString;

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

        public IReadOnlyCollection<string> GetTableNames()
        {
            var tables = new List<string>();
            using (var dataReader = GetDataReaderForSql("SHOW TABLES")) {
                while (dataReader.Read()) {
                    string tableName = dataReader.GetString(0);
                    tables.Add(tableName);
                }
            }
            return tables.AsReadOnly();
        }
    }
}
