using System;

using Korzh.DbUtils.Import;
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{
    public enum DbBackupFormat {
        JSON,

        XML
    }


    public class DbInitializer
    {
        private readonly DbImporter _dbImporter;

        public DbInitializer(IDbWriter dbBridge, DbBackupFormat format = DbBackupFormat.JSON, bool zip = false)
        {
            IDatasetImporter datasetImporter;
            if (format == DbBackupFormat.XML) {
                datasetImporter = new XmlDatasetImporter();
            }
            else {
                datasetImporter = new JsonDatasetImporter();
            }

            var unpacker = new FileFolderPacker("App_Data");

            _dbImporter = new DbImporter(dbBridge, datasetImporter, unpacker);
        }

        public void InitDb()
        {
            _dbImporter.Import();
        }
    }

}
