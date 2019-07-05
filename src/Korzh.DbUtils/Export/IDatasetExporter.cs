using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Korzh.DbUtils.Export
{
    public interface IDatasetExporter
    {
        void Export(IDataReader reader, Stream outStream, string datasetName = null);
    }
}
