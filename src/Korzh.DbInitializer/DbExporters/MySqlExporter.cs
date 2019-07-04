using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using MySql.Data.MySqlClient;

using Korzh.DbUtils.DbSavers;

namespace Korzh.DbUtils.Exporters
{
    public class MySqlExporter : DbExporterBase
    {
        public MySqlExporter(string connectionString, IDbSaver saver) : base(connectionString, saver)
        {
            DbConnection = new MySqlConnection(connectionString);
        }

        protected override IReadOnlyCollection<string> GetTables()
        {
            var sql = "SHOW TABLES";

            var tables = new List<string>();
            using (var mySqlCommand = DbConnection.CreateCommand())
            {
                mySqlCommand.CommandText = sql;
                mySqlCommand.CommandType = CommandType.Text;

                using (var dataReader = mySqlCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        string tableName = dataReader.GetString(0);
                        tables.Add(tableName);
                    }
                }

            }

            return tables.AsReadOnly();
        }

        protected override IDataReader ReadTable(string tableName)
        {
            var sql = "SELECT * FROM `" + tableName + "`";

            IDataReader reader = null;
            var command = DbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            reader = command.ExecuteReader();

            return reader;
        }
    }
}
