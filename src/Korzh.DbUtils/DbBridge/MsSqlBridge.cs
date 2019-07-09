using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;


namespace Korzh.DbUtils.DbBridges
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

        protected override string GenerateInsertStatement(string tableName, IDataRecord record)
        {
            WriteToConsole(record);
            return "";
        }

        protected override void TurnOffContraints()
        {
            using (var command = Connection.CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable ""ALTER TABLE ? NOCHECK CONSTRAINT all""";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnContraints()
        {
            using (var command = Connection.CreateCommand()) {
                command.CommandText = @"EXRC sp_MSforeachtable ""ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all""";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOffAutoIncrement()
        {
            using (var command = Connection.CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable @command1=""SET IDENTITY_INSERT ? OFF"",
                                       @whereand = ' AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = o.id  AND is_identity = 1)'";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnAutoIncrement()
        {
            using (var command = Connection.CreateCommand()) {
                command.CommandText = @"EXEC sp_MSforeachtable @command1=""SET IDENTITY_INSERT ? ON"",
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
