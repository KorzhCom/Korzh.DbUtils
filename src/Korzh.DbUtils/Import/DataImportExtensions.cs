using System.Collections.Generic;
using System.IO;

using Korzh.DbUtils.Import;

namespace Korzh.DbUtils
{
    public static class DataImportExtensions
    {
        public static void UseJsonImporter(this DbInitializerOptions options)
        {
            options.DatasetImporter = new JsonDatasetImporter();
        }

        public static void UseXmlImporter(this DbInitializerOptions options)
        {
            options.DatasetImporter = new XmlDatasetImporter();
        }


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
