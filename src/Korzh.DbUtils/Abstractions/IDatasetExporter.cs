using System.Data;
using System.IO;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Defines methods and properties for a dataset importer
    /// </summary>
    public interface IDatasetExporter
    {
        /// <summary>
        /// Exports one dataset (table) to a stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="outStream">The output stream.</param>
        /// <param name="dataset">
        /// An instance of <see cref="DatasetInfo"/> class which represents basic table information.
        /// Can be ommitted if you export only one dataset
        /// </param>
        void ExportDataset(IDataReader reader, Stream outStream, DatasetInfo dataset = null);

        /// <summary>
        /// Gets the default file extension for the current exporting format.
        /// </summary>
        /// <value>The default file extension.</value>
        string FileExtension { get; }
    }
}
