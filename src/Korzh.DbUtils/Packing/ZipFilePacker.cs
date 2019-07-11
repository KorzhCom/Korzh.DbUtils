using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    /// <summary>
    /// Implements <see cref="Korzh.DbUtils.IDataPacker" /> and <see cref="Korzh.DbUtils.IDataUnpacker" /> interfaces
    /// to store the data as a ZIP archive file. Each packed entry - is one file in that archive.
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDataPacker" />
    /// <seealso cref="Korzh.DbUtils.IDataUnpacker" />
    public class ZipFilePacker : IDataPacker, IDataUnpacker
    {
        private readonly string _filePath;
        private readonly ILogger _logger; 
        private ZipArchive _zipArchive;
        private string _fileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFilePacker"/> class.
        /// </summary>
        /// <param name="filePath">The path to the ZIP file.</param>
        /// <param name="logger">The logger.</param>
        public ZipFilePacker(string filePath, ILogger logger = null)
        {
            _filePath = filePath;

            if (!_filePath.EndsWith(".zip")) {
                _filePath += ".zip";
            }

            _logger = logger;
        }

        /// <summary>
        /// Starts the packing process.
        /// </summary>
        /// <param name="fileExtension">The extension packed entry (file).
        /// Can be an empty string if we don't pack in files (e.g. we store to some DB)</param>
        public void StartPacking(string fileExtension)
        {
            _fileExtension = fileExtension;
            _zipArchive = ZipFile.Open(_filePath, ZipArchiveMode.Update);
            var entryNames = _zipArchive.Entries.Select(e => e.Name).ToList();
            foreach (var name in entryNames)
            {
                var entry = _zipArchive.GetEntry(name);
                entry?.Delete();
            }

            _logger?.LogInformation("Start writting to file: " + _filePath);
        }

        /// <summary>
        /// Opens the stream for packing.
        /// </summary>
        /// <param name="entryName">The name of the entry to pack.</param>
        /// <returns>Stream.</returns>
        public Stream OpenStreamForPacking(string entryName)
        {
            var entry = _zipArchive.CreateEntry(entryName + "." + _fileExtension);
            return entry.Open();
        }

        /// <summary>
        /// Finishes the packing. Flushes the data and closes the ZIP archive stream.
        /// </summary>
        public void FinishPacking()
        {
            _zipArchive.Dispose();

            _logger?.LogInformation("Finish writting to file: " + _filePath);
        }

        /// <summary>
        /// Starts the unpacking. Opens the ZIP arhive file.
        /// </summary>
        /// <param name="fileExtension">The default file extension used for packed entries (files).</param>
        public void StartUnpacking(string fileExtension)
        {
            _zipArchive = ZipFile.Open(_filePath, ZipArchiveMode.Read);
            _fileExtension = fileExtension;

            _logger?.LogInformation("Start unpacking" + _filePath);
        }

        /// <summary>
        /// Finishes the unpacking. Does nothing in this case.
        /// </summary>
        public void FinishUnpacking()
        {
            _logger?.LogInformation("Finish unpacking" + _filePath);
        }

        /// <summary>
        /// Opens the stream for one entry we are going to unpack.
        /// </summary>
        /// <param name="entryName">Name of the entry.</param>
        /// <returns>Stream.</returns>
        public Stream OpenStreamForUnpacking(string entryName)
        {
            var fileName = entryName + "." + _fileExtension;
            var entry = _zipArchive.Entries.FirstOrDefault(e => e.Name == fileName);
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
