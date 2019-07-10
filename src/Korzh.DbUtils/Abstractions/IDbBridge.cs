using System.Collections.Generic;
using System.Data;

namespace Korzh.DbUtils
{
    public interface IDbBridge
    {
        IDbConnection GetConnection();

        IReadOnlyCollection<DatasetInfo> GetDatasets();
    }

    public interface IDbReader : IDbBridge
    {

        IDataReader GetDataReaderForTable(string tableName);

        IDataReader GetDataReaderForSql(string sql);
    }

    public interface IDbWriter : IDbBridge
    {
        void WriteRecord(IDataRecord record);

        void StartSeeding(DatasetInfo table);

        void FinishSeeding();
    }
}
