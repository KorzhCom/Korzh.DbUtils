using System;
using System.IO;
using System.Data;

namespace Korzh.DbUtils
{
    public interface IDatasetImporter
    {
        void StartImport(Stream datasetStream);

        bool HasRecords();

        IDataRecord NextRecord();

        void FinishImport();
    }

    public class DatasetImporterException : Exception
    {
        public DatasetImporterException(string message) : base(message)
        {
        }
    }
}
