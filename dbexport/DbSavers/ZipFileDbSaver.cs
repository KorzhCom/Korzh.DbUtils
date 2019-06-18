using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

using Microsoft.Extensions.Logging;

namespace dbexport.DbSavers
{
    internal abstract class ZipFileDbSaver: IDbSaver
    {
        private readonly string _fileName;
        protected ILogger Logger; 

        protected ZipArchive ZipArchive;

        public ZipFileDbSaver(string fileName, ILogger logger)
        {
            _fileName = fileName;

            if (!_fileName.EndsWith(".zip"))
                _fileName += ".zip";

            Logger = logger;
        }

        public void Start()
        {
            ZipArchive = ZipFile.Open(_fileName, ZipArchiveMode.Update);

            Logger?.LogInformation("Start writting to file: " + _fileName);
        }

        public abstract void StartSaveTable(string tableName);

        public abstract void SaveTableData(IDataReader dataReader);

        public abstract void EndSaveTable();

        public void End()
        {
            ZipArchive.Dispose();

            Logger?.LogInformation("Finish writting to file: " + _fileName);
        }
    }
}
