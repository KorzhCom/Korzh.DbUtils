using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Korzh.DbUtils
{
    public interface IDbBridge
    {
        IDbConnection GetConnection();

        IReadOnlyCollection<string> GetTableNames();

        IDataReader GetDataReaderForTable(string tableName);

        IDataReader GetDataReaderForSql(string sql);

        void WriteRecord(DataRow record);
    }
}
