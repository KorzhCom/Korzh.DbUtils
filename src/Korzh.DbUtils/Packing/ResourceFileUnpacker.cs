using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Packing
{
    /// <summary>
    /// Implements <see cref="Korzh.DbUtils.IDataUnpacker" /> interface
    /// to load the data as files from embeded resource. Each packed entry - is one file.
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDataUnpacker" />
    public class ResourceFileUnpacker : IDataUnpacker
    {

        private readonly Assembly _assembly;
        private readonly string _folderPath = "";
        private ILogger _logger;
        private string _fileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFileUnpacker"/> class.
        /// </summary>
        /// <param name="assembly">The assembly with embedded resources.</param>
        public ResourceFileUnpacker(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFileUnpacker"/> class.
        /// </summary>
        /// <param name="assembly">The assembly with embedded resources.</param>
        /// <param name="folderPath">The folder in resources.</param>
        public ResourceFileUnpacker(Assembly assembly, string folderPath): this(assembly)
        {
            _folderPath = folderPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFileUnpacker"/> class.
        /// </summary>
        /// <param name="assembly">The assembly with embedded resources.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ResourceFileUnpacker(Assembly assembly, ILoggerFactory loggerFactory) : this(assembly)
        {
            _logger = loggerFactory.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFileUnpacker"/> class.
        /// </summary>
        /// <param name="assembly">The assembly with embedded resources.</param>
        /// <param name="folderPath">The folder in resources.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ResourceFileUnpacker(Assembly assembly, string folderPath, ILoggerFactory loggerFactory) : this(assembly, folderPath)
        {
            _logger = loggerFactory.CreateLogger("DbUtils.Packing");
        }

        /// <summary>
        /// Starts the unpacking.
        /// </summary>
        /// <param name="fileExtension">The default file extension used for packed entries (files).</param>
        public void StartUnpacking(string fileExtension)
        {
            _fileExtension = fileExtension;

            _logger?.LogDebug("Start unpacking from assembly: " + _assembly.FullName);
        }

        /// <summary>
        /// Finishes the unpacking.
        /// </summary>
        public void FinishUnpacking()
        {
            _logger?.LogDebug("Finish unpacking from assembly: " + _assembly.FullName);
        }

        /// <summary>
        /// Opens the stream for one entry we are going to unpack.
        /// </summary>
        /// <param name="entryName">The name of the entry to unpack.</param>
        /// <returns>Stream.</returns>
        public Stream OpenStreamForUnpacking(string entryName)
        {
            var nameWithFolder = Path.Combine(_folderPath, entryName).Replace('\\', '.').Replace('/', '.');
            var fullName = _assembly.GetManifestResourceNames().FirstOrDefault(res => res.EndsWith(nameWithFolder));

            return (!string.IsNullOrEmpty(fullName))
                   ? _assembly.GetManifestResourceStream(fullName)
                   : null;
        }
    }
}
