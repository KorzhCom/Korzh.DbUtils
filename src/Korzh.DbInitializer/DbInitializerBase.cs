using System;
using System.Collections.Generic;

using Korzh.DbInitializer.Loaders;

namespace Korzh.DbInitializer
{
    public abstract class DbInitializerBase: IDbInitializer
    {

        private readonly IDbInitializerLoader _loader;

        public DbInitializerBase(IDbInitializerLoader loader)
        {
            _loader = loader;
        }

        public void Init()
        {
            var tables = GetTablesInRightOrder();
            foreach (var table in tables)
            {
                var data = _loader.LoadTableData(table);
                InitTable(table, data);
            }
        }

        protected abstract void InitTable(string tableName, IEnumerable<IDataItem> data);

        protected abstract IReadOnlyCollection<string> GetTablesInRightOrder();

    }

}
