using System;
using System.Collections.Generic;

namespace Korzh.DbInitializer
{

    public interface IDbInitializerLoader
    {
        IEnumerable<IDataItem> LoadEntityData(string entityName);

        IEnumerable<TEntity> LoadEntityData<TEntity>(string entityName) where TEntity : class;

        IEnumerable<object> LoadEntityData(string entityName, Type entityType);
    }
}
