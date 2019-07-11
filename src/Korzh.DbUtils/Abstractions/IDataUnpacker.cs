using System.IO;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Defines funvtions for data unpacking operations
    /// </summary>
    public interface IDataUnpacker
    {
        /// <summary>
        /// Starts the unpacking. 
        /// The exact implemention "knows" where to find the packed data.
        /// </summary>
        /// <param name="fileExtension">
        /// The default file extension used for packed entries (files).
        /// This parameter can be null or empty if each archive entry is not represented by some file.
        /// </param>
        void StartUnpacking(string fileExtension);

        /// <summary>
        /// Finishes the unpacking.
        /// </summary>
        void FinishUnpacking();

        /// <summary>
        /// Opens the stream for one entry we are going to unpack.
        /// </summary>
        /// <param name="entryName">The name of the entry (usually a table representation in some format) to unpack.</param>
        /// <returns>Stream.</returns>
        Stream OpenStreamForUnpacking(string entryName);
    }
}
