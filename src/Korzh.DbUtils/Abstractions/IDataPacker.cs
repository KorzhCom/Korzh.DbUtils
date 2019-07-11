using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Defines basic data packing operations
    /// </summary>
    public interface IDataPacker
    {
        /// <summary>
        /// Starts the packing process.
        /// </summary>
        /// <param name="fileExtension">The extension packed entry (file). 
        /// Can be an empty string if we don't pack in files (e.g. we store to some DB)
        /// </param>
        void StartPacking(string fileExtension);

        /// <summary>
        /// Opens and returns the writing stream for some packing entry.
        /// </summary>
        /// <param name="entryName">Name of the entry to pack.</param>
        /// <returns>Stream.</returns>
        Stream OpenStreamForPacking(string entryName);

        /// <summary>
        /// Finishes the packing.
        /// Use this operation to flush the data, close used streams, etc
        /// </summary>
        void FinishPacking();
    }

    /// <summary>
    /// Represents errors that occur during packing/unpacking operations.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DatapackingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetImporterException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DatapackingException(string message) : base(message)
        {
        }
    }
}
