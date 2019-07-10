using System;
using System.Data;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Korzh.DbUtils.Export
{
    public class JsonDatasetExporter: IDatasetExporter
    {
        private readonly ILogger _logger = null;

        public JsonDatasetExporter()
        {          
        }

        public JsonDatasetExporter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        public string FileExtension => "json";

        public void ExportDataset(IDataReader dataReader, Stream outStream, DatasetInfo dataset = null)
        {
            _logger?.LogInformation("Start saving dataset: " + dataset?.Name);

            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            using (var writer = new JsonTextWriter(new StreamWriter(outStream, Encoding.UTF8))) { ///TODO: Set encoding elsewhere
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();  //root object start
                writer.WritePropertyName("name");
                writer.WriteValue(dataset?.Name);

                writer.WritePropertyName("schema");
                writer.WriteStartObject();  //schema object start
                WriteSchemaProperties(writer, dataReader);
                writer.WriteEndObject();    //schema object end

                writer.WritePropertyName("data");
                writer.WriteStartArray();   //data array start

                while (dataReader.Read()) {
                    _logger?.LogDebug("Start writting row.");

                    writer.WriteStartObject();
                    foreach (var column in columns) {
                        var value = dataReader.GetValue(dataReader.GetOrdinal(column));
                        if (value.GetType() != typeof(DBNull)) {
                            writer.WritePropertyName(column);
                            writer.WriteValue(value);

                            _logger?.LogDebug($"Column={column}; Value={value}");
                        }
                    }
                    writer.WriteEndObject();

                    _logger?.LogDebug("Finish writting row.");
                }
                writer.WriteEndArray();     //data array end
                writer.WriteEndObject();    //root object end

                _logger?.LogInformation("Finish saving dataset: " + dataset?.Name);
            }
        }

        protected virtual void WriteSchemaProperties(JsonWriter writer, IDataReader dataReader)
        {
            writer.WritePropertyName("columns");
            writer.WriteStartArray(); //data fields start
            var schema = dataReader.GetSchemaTable();
            foreach (DataRow row in schema.Rows) {
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(row["ColumnName"].ToString());

                writer.WritePropertyName("type");
                writer.WriteValue(row["DataType"].ToString());

                writer.WriteEndObject();
            }
            writer.WriteEndArray(); //data fields end
        }
    }
}
