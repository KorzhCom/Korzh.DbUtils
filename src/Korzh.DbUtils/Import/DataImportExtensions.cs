using System;

using Korzh.DbUtils;
using Korzh.DbUtils.Import;

namespace Microsoft.Extensions.DependencyInjection
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
    }
}
