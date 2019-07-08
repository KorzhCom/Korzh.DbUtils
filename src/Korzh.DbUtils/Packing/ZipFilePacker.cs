using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    public class ZipFilePacker : IDataPacker, IDataUnpacker
    {
        private readonly string _fileName;
        private ILogger _logger; 
        private ZipArchive _zipArchive;

        public ZipFilePacker(string fileName, ILogger logger = null)
        {
            _fileName = fileName;

            if (!_fileName.EndsWith(".zip")) {
                _fileName += ".zip";
            }

            _logger = logger;
        }

        public void StartPacking()
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

        public void FinishPacking()
        {
            _zipArchive.Dispose();

            _logger?.LogInformation("Finish writting to file: " + _fileName);
        }

        //private IEnumerable<ZipArchiveEntry> _unpackingEntries;

        private int _unpackingEntry = 0;

        public void StartUnpacking()
        {
            _zipArchive = ZipFile.Open(_fileName, ZipArchiveMode.Read);
            _unpackingEntry = 0;

            //_unpackingEntries = _zipArchive.Entries;//.Select(e => e.Name).ToList();
            //foreach (var name in entryNames) {
            //    var entry = _zipArchive.GetEntry(name);
            //    entry?.Delete();
            //}

            _logger?.LogInformation("Start unpacking" + _fileName);
        }

        public void FinishUnpacking()
        {
            _logger?.LogInformation("Finish unpacking" + _fileName);
        }

        public bool HasData()
        {
            return _unpackingEntry < _zipArchive.Entries.Count;
        }

        public Stream NextDatasetStream()
        {
            if (!HasData()) {
                throw new ZipFilePackerException("No more entries to unpack");
            }
            var entry = _zipArchive.Entries[_unpackingEntry];
            return entry.Open();
        }
    }

    public class ZipFilePackerException : Exception
    {
        public ZipFilePackerException(string message) : base(message)
        {
        }
    }
}
