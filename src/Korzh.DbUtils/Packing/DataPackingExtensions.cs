
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Contains some useful extension methods for registering data packers/unpackers in <see cref="DbInitializerOptions"/> .
    /// </summary>
    public static class DataPackingExtensions
    {
        /// <summary>
        /// Registers an instance of <see cref="FileFolderPacker"/> class as unpacker for DbInitializer.
        /// </summary>
        /// <param name="options">An instance of <see cref="DbInitializerOptions"/>.</param>
        /// <param name="folderPath">The path to the folder where the data files (the backup) are stored.</param>
        public static void UseFileFolderPacker(this DbInitializerOptions options, string folderPath = null)
        {
            if (string.IsNullOrEmpty(folderPath)) {
                folderPath = options.InitialDataFolder;
            }
            options.Unpacker = new FileFolderPacker(folderPath);
        }

        /// <summary>
        /// Registers an instance of <see cref="ZipFilePacker"/> class as unpacker for DbInitializer.
        /// </summary>
        /// <param name="options">An instance of <see cref="DbInitializerOptions"/>.</param>
        /// <param name="folderPath">The path to the file where the ZIP archive is stored. 
        /// If null - `dataseed.zip` file will be used.</param>
        public static void UseZipPacker(this DbInitializerOptions options, string zipFilePath = null)
        {
            if (string.IsNullOrEmpty(zipFilePath)) {
                zipFilePath = System.IO.Path.Combine(options.InitialDataFolder, "dataseed.zip");
            }
            options.Unpacker = new ZipFilePacker(zipFilePath);
        }
    }
}
