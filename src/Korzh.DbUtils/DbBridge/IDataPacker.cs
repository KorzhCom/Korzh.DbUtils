using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Korzh.DbUtils
{
    public interface IDataPacker
    {
        void StartPacking();

        Stream OpenStreamForPacking(string datasetName);

        void FinishPacking();
    }
}
