using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.SqlServer
{
    public class SqlServerBridge : BaseDbBridge
    {
        public SqlServerBridge(string connectionString) : base(connectionString)
        {
        }

        public SqlServerBridge(string connectionString, ILoggerFactory loggerFactory) : base(connectionString, loggerFactory)
        {
        }

        public SqlServerBridge(SqlConnection connection) : base(connection)
        {
        }

        public SqlServerBridge(SqlConnection connection, ILoggerFactory loggerFactory) : base(connection, loggerFactory)
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
                    SqlDbType = record.GetFieldType(i).ToSqlDbType()
                };

                command.Parameters.Add(parameter);
            }
        }

        protected override void TurnOffConstraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"ALTER TABLE {GetTableFullName(CurrentSeedingTable)} NOCHECK CONSTRAINT all";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnConstraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"ALTER TABLE {GetTableFullName(CurrentSeedingTable)} CHECK CONSTRAINT all";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOffAutoIncrement()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"IF EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = object_id('{GetTableFullName(CurrentSeedingTable)}') AND c.is_identity =1) begin SET IDENTITY_INSERT {GetTableFullName(CurrentSeedingTable)} ON end";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnAutoIncrement()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"IF EXISTS (SELECT 1 from sys.columns c WHERE c.object_id = object_id('{GetTableFullName(CurrentSeedingTable)}') AND c.is_identity = 1) begin SET IDENTITY_INSERT {GetTableFullName(CurrentSeedingTable)} OFF end";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }
    }
}
