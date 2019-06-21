using System;
using System.Collections.Generic;

namespace Korzh.DbInitializer.Loaders
{

    public interface IDbInitializerLoader
    {
        IEnumerable<IDataItem> LoadTableData(string tableName);
    }
}
