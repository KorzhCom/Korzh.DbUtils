using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.DbBridges
{
    //public class MySqlBride : BaseDbBridge
    //{
    //    public MySqlBride(string connectionString) : base(connectionString)
    //    {
    //    }

    //    public MySqlBride(MySqlConnection connection) : base(connection)
    //    {
    //    }

    //    protected override DbConnection CreateConnection(string connectionString)
    //    {
    //        return new MySqlConnection(connectionString);
    //    }

    //    protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
    //    {
    //        using (var dataReader = GetDataReaderForSql("SHOW TABLES")) {
    //            while (dataReader.Read()) {
    //                string tableName = dataReader.GetString(0);
    //                datasets.Add(new DatasetInfo(tableName));
    //            }
    //        }
    //    }
    //}

}
