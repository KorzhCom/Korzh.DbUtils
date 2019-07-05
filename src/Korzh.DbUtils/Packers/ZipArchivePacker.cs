using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packers
{
    public class ZipArchivePacker : IDataPacker
    {
        private readonly string _fileName;
        private ILogger _logger; 
        private ZipArchive _zipArchive;

        public ZipArchivePacker(string fileName, ILogger logger = null)
        {
            _fileName = fileName;

            if (!_fileName.EndsWith(".zip")) {
                _fileName += ".zip";
            }

            _logger = logger;
        }

        public void Start()
        {
            _zipArchive = ZipFile.Open(_fileName, ZipArchiveMode.Update);
            var entryNames = _zipArchive.Entries.Select(e => e.Name).ToList();
            foreach (var name in entryNames)
            {
                var entry = _zipArchive.GetEntry(name);
                entry?.Delete();
            }

            _logger?.LogInformation("Start writting to file: " + _fileName);
        }

        public Stream OpenStream(string entryName)
        {
            var entry = _zipArchive.CreateEntry(entryName);
            return entry.Open();
        }

        public void Finish()
        {
            _zipArchive.Dispose();

            _logger?.LogInformation("Finish writting to file: " + _fileName);
        }
    }
}
