using System.IO;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    /// <summary>
    /// Implements <see cref="Korzh.DbUtils.IDataPacker" /> and <see cref="Korzh.DbUtils.IDataUnpacker" /> interfaces
    /// to store the data as files in some folder. Each packed entry - is one file.
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDataPacker" />
    /// <seealso cref="Korzh.DbUtils.IDataUnpacker" />
    public class FileFolderPacker : IDataPacker, IDataUnpacker
    {
        private readonly string _folderPath;
        private ILogger _logger;
        private string _fileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFolderPacker"/> class.
        /// </summary>
        /// <param name="folderPath">The path to folder where the files will be stored (or from which they will be taken in case of unpacker).</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public FileFolderPacker(string folderPath, ILoggerFactory loggerFactory = null)
        {
            _folderPath = folderPath;

            _logger = loggerFactory?.CreateLogger("DbUtils.Packing");
        }

        /// <summary>
        /// Starts the packing process.
        /// </summary>
        /// <param name="fileExtension">The extension packed entry (file).
        /// Can be an empty string if we don't pack in files (e.g. we store to some DB)</param>
        public void StartPacking(string fileExtension)
        {
            _logger?.LogDebug("Start writing to folder: " + _folderPath);
            Directory.CreateDirectory(_folderPath);
            _fileExtension = fileExtension;
        }

        /// <summary>
        /// Opens the stream for packing.
        /// </summary>
        /// <param name="entryName">Name of the entry (dataset) to pack.</param>
        /// <returns>Stream.</returns>
        public Stream OpenStreamForPacking(string entryName)
        {
            var filePath = Path.Combine(_folderPath, entryName + "." + _fileExtension);
            return File.Create(filePath);
        }

        /// <summary>
        /// Finishes the packing.
        /// Use this operation to flush the data, close used streams, etc
        /// </summary>
        public void FinishPacking()
        {
            _logger?.LogDebug("Finished writing to folder: " + _folderPath);
        }

        /// <summary>
        /// Starts the unpacking.
        /// </summary>
        /// <param name="fileExtension">The default file extension used for packed entries (files).</param>
        public void StartUnpacking(string fileExtension)
        {
            _fileExtension = fileExtension;
            if (!Directory.Exists(_folderPath)) {
                throw new DataPackingException("No such folder:" + _folderPath);
            }
        }

        /// <summary>
        /// Finishes the unpacking.
        /// </summary>
        public void FinishUnpacking()
        {
        }

        /// <summary>
        /// Opens the stream for one entry we are going to unpack.
        /// </summary>
        /// <param name="entryName">The name of the entry to unpack.</param>
        /// <returns>Stream.</returns>
        public Stream OpenStreamForUnpacking(string entryName)
        {
            var filePath = Path.Combine(_folderPath, entryName + "." + _fileExtension);
            return File.Exists(filePath) 
                ? File.OpenRead(filePath) 
                : null;
        }
    }
}
