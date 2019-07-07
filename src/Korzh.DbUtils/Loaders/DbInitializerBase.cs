using System;
using System.Collections.Generic;
using System.Linq;

using Korzh.DbUtils.Loaders;

namespace Korzh.DbUtils
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
            var tables = GetTables();

            //Init table cycle
            foreach (var table in tables)
            {
                try
                {
                    var data = _loader.LoadTableData(table);
                    InitTable(table, data);
                }
                catch (ZipFileLoaderException ex)
                {
                    // Nothing to do
                }
                
            }
        }

        protected abstract void InitTable(string tableName, IEnumerable<IDataRecord> data);

        protected abstract IReadOnlyCollection<string> GetTables();

    }

}
