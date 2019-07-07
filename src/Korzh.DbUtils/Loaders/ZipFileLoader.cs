using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Korzh.DbUtils.Loaders
{

    public abstract class ZipFileLoaderException : Exception
    {

        public ZipFileLoaderException() : base()
        {

        }

        public ZipFileLoaderException(string message) : base(message)
        {

        }
    }

    public abstract class ZipFileLoader : IDisposable
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

        public abstract IEnumerable<IDataRecord> LoadTableData(string entityName);

        public void Dispose()
        {
            ZipArchive.Dispose();
        }
    }
}
