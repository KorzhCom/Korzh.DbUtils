﻿using System.IO;

namespace Korzh.DbUtils.Packing
{
    public interface IDataUnpacker
    {
        void StartUnpacking(string fileExtension);

        void FinishUnpacking();

        bool HasData();

        Stream OpenStreamForUnpacking(string datasetName);
    }

}