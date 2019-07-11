using System;
using System.IO;
using System.Data;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Defines the interface for a dataset importer.
    /// Each implementation of this interface "understands" a particular data format (XML, JSON, CSV, etc)
    /// </summary>
    public interface IDatasetImporter
    {
        /// <summary>
        /// Starts the importing process.
        /// This function processes the first part of the dataset stream and collect necessary information about the dataset
        /// </summary>
        /// <param name="datasetStream">The dataset stream.</param>
        /// <returns>
        /// An instance of the <see cref="DatasetInfo"/> which contains some basic information about the dataset (table).
        /// </returns>
        DatasetInfo StartImport(Stream datasetStream);

        /// <summary>
        /// Determines whether there are more records to process in the input stream.
        /// </summary>
        /// <returns><c>true</c> if this the input stream still has more records for the current dataset; otherwise, <c>false</c>.</returns>
        bool HasRecords();

        /// <summary>
        /// Extracts the next record from the input stream.
        /// </summary>
        /// <returns>IDataRecord.</returns>
        IDataRecord NextRecord();

        /// <summary>
        /// Finilizing the importing process.
        /// </summary>
        void FinishImport();

        /// <summary>
        /// Gets the default file extension for the data format processed by this importer (e.g "xml" or "json").
        /// </summary>
        /// <value>The file extension.</value>
        string FileExtension { get; }
    }

    /// <summary>
    /// Represents errors that occur during dataset importing operations.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DatasetImporterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetImporterException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DatasetImporterException(string message) : base(message)
        {
        }
    }
}
