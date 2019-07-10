using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.MySql
{
    public class MySqlBride : BaseDbBridge
    {
        public MySqlBride(string connectionString) : base(connectionString)
        {
        }

        public MySqlBride(string connectionString, ILoggerFactory loggerFactory) : base(connectionString, loggerFactory)
        {
        }

        public MySqlBride(MySqlConnection connection) : base(connection)
        {
        }

        public MySqlBride(MySqlConnection connection, ILoggerFactory loggerFactory) : base(connection, loggerFactory)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            using (var dataReader = GetDataReaderForSql("SHOW TABLES")) {
                while (dataReader.Read()) {
                    string tableName = dataReader.GetString(0);
                    datasets.Add(new DatasetInfo(tableName, ""));
                }
            }
        }
        protected override string Quote1 => "`";

        protected override string Quote2 => "`";

        protected override void AddParameters(IDbCommand command, IDataRecord record)
        {
            for (int i = 0; i < record.FieldCount; i++) {
                var parameter = new MySqlParameter(ToParameterName(record.GetName(i)), record.GetValue(i)) {
                    Direction = ParameterDirection.Input,
                    MySqlDbType = record.GetFieldType(i).ToMySqlDbType()
                };

                command.Parameters.Add(parameter);
            }
        }

        protected override void TurnOffAutoIncrement()
        {
            // NOTHING TO DO: THERE IS NO PROBLEM WITH AUTO_INCREMENT FIELDS
        }

        protected override void TurnOnAutoIncrement()
        {
            // NOTHING TO DO: THERE IS NO PROBLEM WITH AUTO_INCREMENT FIELDS
        }

        protected override void TurnOffConstraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"SET FOREIGN_KEY_CHECKS = 0;";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnConstraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"SET FOREIGN_KEY_CHECKS = 1;";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }
    }

}
