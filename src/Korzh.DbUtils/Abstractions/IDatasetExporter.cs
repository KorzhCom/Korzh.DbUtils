using System.Data;
using System.IO;

/// <summary>
/// Contains all classes which implement exporting operations.
/// </summary>
namespace Korzh.DbUtils
{
    public interface IDatasetExporter
    {
        void ExportDataset(IDataReader reader, Stream outStream, string datasetName = null);

        string FileExtension { get; }
    }
}
