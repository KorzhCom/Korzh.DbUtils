﻿using System;

using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils.Import
{

    public class DbImporter
    {
        private readonly IDbWriter _dbWriter;
        private readonly IDatasetImporter _datasetImporter;
        private readonly IDataUnpacker _dataUnpacker;

        public DbImporter(IDbWriter dbWriter, IDatasetImporter datasetImporter, IDataUnpacker unpacker)
        {
            _dbWriter = dbWriter;
            _datasetImporter = datasetImporter;
            _dataUnpacker = unpacker;
        }

        public void Import()
        {
            _dataUnpacker.StartUnpacking(_datasetImporter.FileExtension);
            try {
                var datasets = _dbWriter.GetDatasets();
                foreach (var table in datasets) {
                    using (var datasetStream = _dataUnpacker.OpenStreamForUnpacking(table.Name)) {
                        var dataset = _datasetImporter.StartImport(datasetStream);
                        Console.WriteLine($"Reading {dataset.Name}..."); //!!!!!!!!!!!!!!!!!!!!!
                        while (_datasetImporter.HasRecords()) {
                            _dbWriter.WriteRecord(table.Name, _datasetImporter.NextRecord());
                        }
                        _datasetImporter.FinishImport();
                        Console.WriteLine("");
                    }
                }
                //while (_dataUnpacker.HasData()) {

                //}
            }
            finally {
                _dataUnpacker.FinishUnpacking();
            }
        }
    }

}
