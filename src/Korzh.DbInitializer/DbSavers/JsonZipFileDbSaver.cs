using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Korzh.DbInitializer.DbSavers
{
    public class JsonZipFileDbSaver : ZipFileDbSaver
    {
        private StreamWriter _fileWriter;
        private JsonWriter _writer;

        public JsonZipFileDbSaver(string fileName, ILogger logger): base(fileName, logger)
        {
          
        }


        public override void StartSaveTable(string tableName)
        {
            Logger?.LogInformation($"Start saving table '{tableName}' to file '{tableName + ".json"}'");

            ZipArchiveEntry entry = ZipArchive.CreateEntry(tableName + ".json");
            _fileWriter = new StreamWriter(entry.Open());

            _writer = new JsonTextWriter(_fileWriter);
            _writer.Formatting = Formatting.Indented;

            _writer.WriteStartArray();
        }

        public override void SaveTableData(IDataReader dataReader)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            while (dataReader.Read())
            {
                Logger?.LogDebug("Start writting row.");

                _writer.WriteStartObject();
                foreach (var column in columns)
                {
                    _writer.WritePropertyName(column);

                    var value = dataReader.GetValue(dataReader.GetOrdinal(column));
                    _writer.WriteValue(value);

                    Logger?.LogDebug($"Column={column}; Value={value}");
                }
                _writer.WriteEndObject();

                Logger?.LogDebug("Finish writting row.");
            }
        }

        public override void EndSaveTable()
        {
            _writer.WriteEndArray();

            _writer.Close();

            _fileWriter.Close();

            Logger?.LogInformation("Finish saving table");
        }
    }
}
