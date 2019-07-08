using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Korzh.DbUtils
{
    public interface IDataPacker
    {
        void StartPacking();

        Stream OpenStream(string datasetName);

        void FinishPacking();
    }
}
