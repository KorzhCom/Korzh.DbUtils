using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Korzh.DbInitializer.Loaders
{
    public abstract class ZipFileLoader : IDbInitializerLoader, IDisposable
    {

        protected ZipArchive ZipArchive;

        public ZipFileLoader(string fileName)
        {
            ZipArchive = ZipFile.Open(fileName, ZipArchiveMode.Read);
        }

        public ZipFileLoader(Stream stream)
        {
            ZipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        }

        public abstract IEnumerable<IDataItem> LoadEntityData(string entityName);

        public abstract IEnumerable<object> LoadEntityData(string entityName, Type entityType);

        public IEnumerable<TEntity> LoadEntityData<TEntity>(string entityName) where TEntity : class
        {
            return (IEnumerable<TEntity>)LoadEntityData(entityName, typeof(TEntity));
        }

        public void Dispose()
        {
            ZipArchive.Dispose();
        }

        protected IReadOnlyDictionary<string, string> GetColumnProperies(Type type)
        {
            var columnProperties = new Dictionary<string, string>();
            foreach (var property in type.GetProperties())
            {
                var attrs = property.GetCustomAttributes(true);
                ColumnAttribute columnAttr = null;

                foreach (var attr in attrs)
                {
                    if (attr is ColumnAttribute)
                    {
                        columnAttr = (ColumnAttribute)attr;
                        break;
                    }
                }

                if (columnAttr != null)
                {
                    columnProperties[columnAttr.Name] = property.Name;
                }
                else
                {
                    columnProperties[property.Name] = property.Name;
                }

            }

            return columnProperties;
        }
    }
}
