using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.DbBridges
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
                    datasets.Add(new DatasetInfo(tableName));
                }
            }
        }

        protected override string GenerateInsertStatement(string tableName, IDataRecord record)
        {
            var sb = new StringBuilder(100);
            sb.AppendFormat("INSERT INTO `0` (", tableName);

            for (var i = 0; i < record.FieldCount; i++) {
                sb.AppendFormat("`{0}`, ", record.GetName(i));
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(") VALUES (");

            for (var i = 0; i < record.FieldCount; i++) {
                //sqlBuilder.AppendFormat("`{0}`, ", );
            }

            //!!!!!!!!!!!!!!!! NOT IMPLEMENTED COMPLETELY YET


            return sb.ToString();
        }
    }
}
