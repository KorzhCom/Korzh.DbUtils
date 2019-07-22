using System.Collections.Generic;
using System.IO;

using Korzh.DbUtils.Import;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Contains some useful extension methods for registering data importers in <see cref="IDbUtilsOptions"/> .
    /// </summary>
    public static class DataImportExtensions
    {
        /// <summary>
        /// Registers an instance of <see cref="JsonDatasetImporter"/> class as dataset importer for DbInitializer.
        /// </summary>
        /// <param name="options">An instance of <see cref="IDbUtilsOptions"/>.</param>
        public static void UseJsonImporter(this IDbUtilsOptions options)
        {
            options.DatasetImporter = new JsonDatasetImporter(options.LoggerFactory);
        }

        /// <summary>
        /// Registers an instance of <see cref="XmlDatasetImporter"/> class as dataset importer for DbInitializer.
        /// </summary>
        /// <param name="options">An instance of <see cref="IDbUtilsOptions"/>.</param>
        public static void UseXmlImporter(this IDbUtilsOptions options)
        {
            options.DatasetImporter = new XmlDatasetImporter(options.LoggerFactory);
        }


        /// <summary>
        /// Stores the content of some dataset (table) to the list.
        /// </summary>
        /// <param name="importer">The importer.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputList">The output list.</param>
        public static void ImportToList(this IDatasetImporter importer, Stream inputStream, IList<DataRecord> outputList)
        {
            DataRecord record = null;
            try {
                var dataInfo = importer.StartImport(inputStream);
                while (importer.HasRecords()) {
                    record = (DataRecord)importer.NextRecord();
                    outputList.Add(record);
                }
            }
            finally {
                importer.FinishImport();
            }
        }
    }
}
