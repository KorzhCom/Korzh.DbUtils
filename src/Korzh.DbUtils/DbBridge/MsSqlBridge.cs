using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Korzh.DbUtils.SqlServer
{
    public class MsSqlBridge : BaseDbBridge
    {
        public MsSqlBridge(string connectionString) : base(connectionString)
        {
        }

        public MsSqlBridge(SqlConnection connection) : base(connection)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            DataTable schemaTable = Connection.GetSchema(SqlClientMetaDataCollectionNames.Tables);

            foreach (DataRow row in schemaTable.Rows) {
                string tableType = (string)row["TABLE_TYPE"];
                string tableName = (string)row["TABLE_NAME"];
                if (tableType == "BASE TABLE") {
                    datasets.Add(new DatasetInfo(tableName));
                }
            }
        }

        protected override void AddParameters(IDbCommand command, IDataRecord record)
        {
  
            for (int i = 0; i < record.FieldCount; i++) {
                var parameter = new SqlParameter(ToParameterName(record.GetName(i)), record.GetValue(i))
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = GetDbTypeByClrType(record.GetFieldType(i))
                };

                command.Parameters.Add(parameter);
            }
        }

        protected SqlDbType GetDbTypeByClrType(Type type)
        {
            if (type.IsBool())
                return SqlDbType.Bit;

            if (type.IsByte())
                return SqlDbType.Char;

            if (type.IsInt16())
                return SqlDbType.SmallInt;

            if (type.IsInt32())
                return SqlDbType.Int;

            if (type.IsInt64())
                return SqlDbType.BigInt;

            if (type.IsFloat() || type.IsDouble())
                return SqlDbType.Float;

            if (type == typeof(string))
                return SqlDbType.Text;

            if (type.IsChar())
                return SqlDbType.Char;

            if (type == typeof(byte[]))
                return SqlDbType.Binary;

            if (type.IsDateTime())
                return SqlDbType.DateTime;

            if (type.IsDateTimeOffset())
                return SqlDbType.DateTime2;

            return SqlDbType.Text;
        
        }

        protected override void TurnOffContraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable ""ALTER TABLE ? NOCHECK CONSTRAINT all""";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnContraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable ""ALTER TABLE ? CHECK CONSTRAINT all""";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOffAutoIncrement()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable @command1=""SET IDENTITY_INSERT ? ON"",
                                       @whereand = ' AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = o.id  AND is_identity = 1)'";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnAutoIncrement()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable @command1=""SET IDENTITY_INSERT ? OFF"",
                                       @whereand = ' AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = o.id  AND is_identity = 1)'";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
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
