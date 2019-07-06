using System.Collections.Generic;
using System.IO;

namespace Korzh.DbUtils.Import
{
    public interface IDatasetImporter
    {
        IEnumerable<IDataItem> ImportDataset(string tableName);
        void StartImport(Stream datasetStream);

        bool HasRecords();
        System.Data.DataRow NextRecord();

        void FinishImport();
    }
}
