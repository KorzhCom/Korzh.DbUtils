using System.Data;
using System.Collections.Generic;

namespace Korzh.DbUtils.Loaders
{
    public interface IDbInitializerLoader
    {
        IEnumerable<IDataRecord> LoadTableData(string tableName);
    }
}
