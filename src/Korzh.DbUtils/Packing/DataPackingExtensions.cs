
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{
    public static class DataPackingExtensions
    {
        public static void UseFileFolderPacker(this DbInitializerOptions options, string folderPath = null)
        {
            if (string.IsNullOrEmpty(folderPath)) {
                folderPath = options.InitialDataFolder;
            }
            options.Unpacker = new FileFolderPacker(folderPath);
        }

        public static void UseZipPacker(this DbInitializerOptions options, string zipFilePath = null)
        {
            if (string.IsNullOrEmpty(zipFilePath)) {
                zipFilePath = System.IO.Path.Combine(options.InitialDataFolder, "dataseed.zip");
            }
            options.Unpacker = new ZipFilePacker(zipFilePath);
        }
    }
}
