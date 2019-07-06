using System.Collections.Generic;

namespace Korzh.DbUtils.Loaders
{
    public interface IDbInitializerLoader
    {
        IEnumerable<IDataItem> LoadTableData(string tableName);
    }
}
