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
                string tableSchema = (string)row["TABLE_SCHEMA"];
                if (tableType == "BASE TABLE") {
                    datasets.Add(new DatasetInfo(tableName, tableSchema));
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

            if (type.IsDecimal())
                return SqlDbType.Decimal;

            if (type == typeof(string))
                return SqlDbType.Text;

            if (type.IsChar())
                return SqlDbType.Char;

            if (type == typeof(byte[]))
                return SqlDbType.VarBinary;

            if (type.IsDateTime())
                return SqlDbType.DateTime;

            if (type.IsDateTimeOffset())
                return SqlDbType.DateTime2;

            return SqlDbType.Text;
        
        }

        protected override void TurnOffContraints(DatasetInfo table)
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"ALTER TABLE {GetTableFullName(table)} NOCHECK CONSTRAINT all";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnContraints(DatasetInfo table)
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"ALTER TABLE {GetTableFullName(table)} CHECK CONSTRAINT all";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOffAutoIncrement(DatasetInfo table)
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"if exists (select 1 from sys.columns c where c.object_id = object_id('{GetTableFullName(table)}') and c.is_identity =1) begin SET IDENTITY_INSERT {GetTableFullName(table)} ON end";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnAutoIncrement(DatasetInfo table)
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"if exists (select 1 from sys.columns c where c.object_id = object_id('{GetTableFullName(table)}') and c.is_identity = 1) begin SET IDENTITY_INSERT {GetTableFullName(table)} OFF end";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }
    }
}
