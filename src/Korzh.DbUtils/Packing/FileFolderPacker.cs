using System;

using System.IO;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    public class FileFolderPacker : IDataPacker
    {
        private readonly string _folderPath;
        private ILogger _logger; 

        public FileFolderPacker(string folderPath, ILogger logger = null)
        {
            _folderPath = folderPath;

            _logger = logger;
        }

        public void StartPacking()
        {
            _logger?.LogInformation("Start writing to folder: " + _folderPath);
            Directory.CreateDirectory(_folderPath);
        }

        public Stream OpenStream(string entryName)
        {
            var filePath = Path.Combine(_folderPath, entryName);
            return File.Create(filePath);
        }

        public void FinishPacking()
        {
            _logger?.LogInformation("Finished writing to folder: " + _folderPath);
        }
    }
}
