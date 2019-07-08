using System;
using System.IO;
using System.Data;

namespace Korzh.DbUtils
{
    public interface IDatasetImporter
    {
        DatasetInfo StartImport(Stream datasetStream);

        bool HasRecords();

        IDataRecord NextRecord();

        void FinishImport();

        string FileExtension { get; }
    }

    public class DatasetImporterException : Exception
    {
        public DatasetImporterException(string message) : base(message)
        {
        }
    }
}
