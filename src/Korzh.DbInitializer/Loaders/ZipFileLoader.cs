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

        public abstract IEnumerable<IDataItem> LoadTableData(string entityName);

        public void Dispose()
        {
            ZipArchive.Dispose();
        }
    }
}
