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
        private ILogger _logger = null;

        public JsonDatasetExporter()
        {          
        }

        public string FileExtension => "json";

        public void ExportDataset(IDataReader dataReader, Stream outStream, string datasetName = null)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            using (var writer = new JsonTextWriter(new StreamWriter(outStream, Encoding.UTF8))) { ///TODO: Set encoding elsewhere
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();  //root object start
                writer.WritePropertyName("name");
                writer.WriteValue(datasetName);

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
                        writer.WritePropertyName(column);

                        var value = dataReader.GetValue(dataReader.GetOrdinal(column));
                        writer.WriteValue(value);

                        _logger?.LogDebug($"Column={column}; Value={value}");
                    }
                    writer.WriteEndObject();

                    _logger?.LogDebug("Finish writting row.");
                }
                writer.WriteEndArray();     //data array end
                writer.WriteEndObject();    //root object end
            }
        }

        protected virtual void WriteSchemaProperties(JsonWriter writer, IDataReader dataReader)
        {
            writer.WritePropertyName("colums");
            writer.WriteStartArray(); //data fields start
            var schema = dataReader.GetSchemaTable();
            foreach (DataColumn column in schema.Columns) {
                writer.WritePropertyName("name");
                writer.WriteValue(column.ColumnName);

                writer.WritePropertyName("type");
                writer.WriteValue(column.DataType.ToString());
            }
            writer.WriteEndArray(); //data fields end
        }
    }
}
