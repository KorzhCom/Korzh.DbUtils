﻿using System.IO;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    public class FileFolderPacker : IDataPacker, IDataUnpacker
    {
        private readonly string _folderPath;
        private ILogger _logger;
        private string _fileExtension;

        public FileFolderPacker(string folderPath, ILoggerFactory loggerFactory = null)
        {
            _folderPath = folderPath;

            _logger = loggerFactory?.CreateLogger("DbUtils.Packing");
        }

        public void StartPacking(string fileExtension)
        {
            _logger?.LogInformation("Start writing to folder: " + _folderPath);
            Directory.CreateDirectory(_folderPath);
            _fileExtension = fileExtension;
        }

        public Stream OpenStreamForPacking(string datasetName)
        {
            var filePath = Path.Combine(_folderPath, datasetName + "." + _fileExtension);
            return File.Create(filePath);
        }

        public void FinishPacking()
        {
            _logger?.LogInformation("Finished writing to folder: " + _folderPath);
        }

        public void StartUnpacking(string fileExtension)
        {
            _fileExtension = fileExtension;
        }

        public void FinishUnpacking()
        {
        }

        public bool HasData()
        {
            return Directory.GetFiles(_folderPath).Length > 0;
        }

        public Stream OpenStreamForUnpacking(string datasetName)
        {
            var filePath = Path.Combine(_folderPath, datasetName + "." + _fileExtension);
            return File.Exists(filePath) 
                ? File.OpenRead(filePath) 
                : null;
        }
    }
}
