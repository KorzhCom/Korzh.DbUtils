using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Korzh.DbUtils.Loaders;

namespace Korzh.DbUtils.EntityFrameworkCore
{

    public class DbContextInitializerException : Exception
    {
        public DbContextInitializerException(string message) : base(message)
        {

        }
    }

    public class DbContextInitializer: DbInitializerBase
    {

        private readonly DbContext _dbContext;

        private readonly int _quotumPerTransaction = 100;

        private readonly IReadOnlyCollection<string> _ignoreTables = new List<string>();

        public DbContextInitializer(DbContext dbContext, IDbInitializerLoader loader) : base(loader)
        {
            _dbContext = dbContext;
        }

        public DbContextInitializer(DbContext dbContext, IDbInitializerLoader loader, IReadOnlyCollection<string> ignoreTables) : this(dbContext, loader)
        {
            _ignoreTables = ignoreTables;
        }

        protected override IReadOnlyCollection<string> GetTables()
        {
            var entityTypes = _dbContext.Model.GetEntityTypes().Where(e => !_ignoreTables.Contains(e.Relational().TableName)).ToList();
            var tables = new List<string>();

            foreach (var entityType in entityTypes)
            {
                if (!tables.Contains(entityType.Relational().TableName))
                    DetermineTableOrder(null, entityType, ref tables);
            }

            tables.Reverse();

            return tables;
        }

        private void DetermineTableOrder(IEntityType startEntityType, IEntityType curEntityType, ref List<string> tables)
        {
            if (startEntityType == curEntityType)
            {
                throw new DbContextInitializerException($"Loop is detected between tables. Unable to find the right order for tables.");
            }

            var refereneces = curEntityType.GetReferencingForeignKeys();
            if (refereneces.Any())
            {
                foreach (var reference in refereneces)
                {
                    var refTableName = reference.DeclaringEntityType.Relational().TableName;
                    if (reference.IsRequired && _ignoreTables.Contains(refTableName))
                    {
                        throw new DbContextInitializerException($"Table references to ignore table. Unable to find the right order for tables.");
                    }

                    if (!tables.Contains(refTableName) && refTableName != curEntityType.Relational().TableName)
                    {
                        DetermineTableOrder(startEntityType, reference.DeclaringEntityType, ref tables);
                    }
                }
            }

            if (!tables.Contains(curEntityType.Relational().TableName))
                tables.Add(curEntityType.Relational().TableName);
        }

        protected override void InitTable(string tableName, IEnumerable<IDataItem> data)
        {
            var entityType = GetEntityTypeByTableName(tableName);
            int count = 0;

            foreach (var dataItem in data)
            {
                var item = Activator.CreateInstance(entityType.ClrType);
                foreach (var property in entityType.GetProperties())
                {
                    if (dataItem.TryGetProperty(property.Relational().ColumnName, property.ClrType, out var propValue))
                    {
                        property.PropertyInfo.SetValue(item, propValue);
                    }
                }

                _dbContext.Add(item);
                count++;

                if (count == _quotumPerTransaction)
                {
                    count = 0;
                    _dbContext.SaveChanges();
                }
            }

            if (count > 0)
            {
                _dbContext.SaveChanges();
            }

        }

        private IEntityType GetEntityTypeByTableName(string tableName)
        {
            var entityTypes = _dbContext.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var mapping = entityType.Relational();
                if (mapping.TableName.Equals(tableName))
                {
                    return entityType;
                }
            }

            return null;
        }
    }

    public class DbContextInitializer<TDbContext> : DbContextInitializer where TDbContext: DbContext
    {
        public DbContextInitializer(TDbContext dbContext, IDbInitializerLoader loader) : base(dbContext, loader)
        {

        }
    }
}
