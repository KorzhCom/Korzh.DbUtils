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
    }

    public interface IDbReader : IDbBridge
    {

        IDataReader GetDataReaderForTable(string tableName);

        IDataReader GetDataReaderForSql(string sql);
    }

    public interface IDbWriter : IDbBridge
    {
        void WriteRecord(string tableName, IDataRecord record);
    }
}
