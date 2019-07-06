using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Korzh.DbUtils.Export
{
    public class JsonDatasetExporter: IDatasetExporter
    {
        private ILogger _logger = null;

        public JsonDatasetExporter()
        {
          
        }

        public string FormatExtension => "json";


        //public override void StartSaveTable(string tableName)
        //{
        //    _logger?.LogInformation($"Start saving table '{tableName}' to file '{tableName + ".json"}'");

        //    ZipArchiveEntry entry = ZipArchive.CreateEntry(tableName + ".json");
        //    _fileWriter = new StreamWriter(entry.Open());
        //}

        public void ExportDataset(IDataReader dataReader, Stream outStream, string datasetName = null)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            using (var writer = new JsonTextWriter(new StreamWriter(outStream, Encoding.UTF8))) { ///TODO: Set encoding elsewhere
                writer.Formatting = Formatting.Indented;

                writer.WriteStartArray();

                while (dataReader.Read()) {
                    _logger?.LogDebug("Start writting row.");

                    writer.WriteStartObject();
                    foreach (var column in columns) {
                        writer.WritePropertyName(column);

                        var value = dataReader.GetValue(dataReader.GetOrdinal(column));
                        writer.WriteValue(value);

                        _logger?.LogDebug($"Column={column}; Value={value}");
                    }
                    writer.WriteEndObject();

                    _logger?.LogDebug("Finish writting row.");
                }
                writer.WriteEndArray();
            }
        }
    }
}
