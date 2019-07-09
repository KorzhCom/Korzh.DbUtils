namespace Korzh.DbUtils.Import
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
