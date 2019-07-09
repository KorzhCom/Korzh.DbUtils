using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.MySql
{
    public class MySqlBride : BaseDbBridge
    {
        public MySqlBride(string connectionString) : base(connectionString)
        {
        }

        public MySqlBride(MySqlConnection connection) : base(connection)
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
                    MySqlDbType = GetDbTypeByClrType(record.GetFieldType(i))
                };

                command.Parameters.Add(parameter);
            }
        }

        protected MySqlDbType GetDbTypeByClrType(Type type)
        {
            if (type.IsBool())
                return MySqlDbType.Bit;

            if (type.IsByte())
                return MySqlDbType.Byte;

            if (type.IsInt16())
                return MySqlDbType.Int16;

            if (type.IsInt32())
                return MySqlDbType.Int32;

            if (type.IsInt64())
                return MySqlDbType.Int64;

            if (type.IsFloat() || type.IsDouble())
                return MySqlDbType.Float;

            if (type.IsDecimal())
                return MySqlDbType.Decimal;

            if (type == typeof(string))
                return MySqlDbType.Text;

            if (type.IsChar())
                return MySqlDbType.VarChar;

            if (type == typeof(byte[]))
                return MySqlDbType.LongBlob;

            if (type.IsDateTime() || type.IsDateTimeOffset())
                return MySqlDbType.DateTime;

            return MySqlDbType.Text;

        }

        protected override void TurnOffAutoIncrement(DatasetInfo table)
        {
            // NOTHING TO DO: THERE IS NO PROBLEM WITH AUTO_INCREMENT FIELDS
        }

        protected override void TurnOnAutoIncrement(DatasetInfo table)
        {
            // NOTHING TO DO: THERE IS NO PROBLEM WITH AUTO_INCREMENT FIELDS
        }

        protected override void TurnOffContraints(DatasetInfo table)
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"SET FOREIGN_KEY_CHECKS = 0;";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnContraints(DatasetInfo table)
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"SET FOREIGN_KEY_CHECKS = 1;";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }
    }

}
