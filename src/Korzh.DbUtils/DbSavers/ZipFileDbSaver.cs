using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.DbSavers
{
    public abstract class ZipFileDbSaver: IDbSaver
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
            var entryNames = ZipArchive.Entries.Select(e => e.Name).ToList();
            foreach (var name in entryNames)
            {
                var entry = ZipArchive.GetEntry(name);
                entry?.Delete();
            }

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
