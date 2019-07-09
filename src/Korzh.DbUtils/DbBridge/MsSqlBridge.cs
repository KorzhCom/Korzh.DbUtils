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

        protected override string GenerateInsertStatement(string tableName, IDataRecord record)
        {
            WriteToConsole(record);
            return "";
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
