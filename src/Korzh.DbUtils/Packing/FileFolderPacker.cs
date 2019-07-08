using System;

using System.IO;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    public class FileFolderPacker : IDataPacker, IDataUnpacker
    {
        private readonly string _folderPath;
        private ILogger _logger; 

        public FileFolderPacker(string folderPath, ILoggerFactory loggerFactory = null)
        {
            _folderPath = folderPath;

            _logger = loggerFactory?.CreateLogger("DbUtils.Packing");
        }

        public void StartPacking()
        {
            _logger?.LogInformation("Start writing to folder: " + _folderPath);
            Directory.CreateDirectory(_folderPath);
        }

        public Stream OpenStreamForPacking(string entryName)
        {
            var filePath = Path.Combine(_folderPath, entryName);
            return File.Create(filePath);
        }

        public void FinishPacking()
        {
            _logger?.LogInformation("Finished writing to folder: " + _folderPath);
        }

        public void StartUnpacking()
        {
        }

        public void FinishUnpacking()
        {
            throw new NotImplementedException();
        }

        public bool HasData()
        {
            throw new NotImplementedException();
        }

        public Stream OpenNextStreamForUnpacking()
        {
            throw new NotImplementedException();
        }
    }
}
