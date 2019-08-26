using System;
using System.Data;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Korzh.DbUtils.Export
{
    /// <summary>
    /// Represents an implmentation of <see cref="Korzh.DbUtils.IDatasetExporter" /> which stores the conten of some dataset to JSON format.
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDatasetExporter" />
    public class JsonDatasetExporter: IDatasetExporter
    {
        private readonly ILogger _logger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDatasetExporter"/> class.
        /// </summary>
        public JsonDatasetExporter()
        {          
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDatasetExporter"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public JsonDatasetExporter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Gets the default file extension for the current exporting format.
        /// </summary>
        /// <value>"json"</value>
        public string FileExtension => "json";

        private static Encoding _uf8Encoding = new UTF8Encoding(false);

        /// <summary>
        /// Exports the dataset's content to JSON
        /// </summary>
        /// <param name="dataReader">The data reader which reads the dataset's content.</param>
        /// <param name="outStream">The output stream.</param>
        /// <param name="dataset">
        /// An instance of <see cref="DatasetInfo"/> class which represents basic table information.
        /// Can be ommitted if you export only one table
        /// </param>
        public void ExportDataset(IDataReader dataReader, Stream outStream, DatasetInfo dataset = null)
        {
            _logger?.LogDebug("Start saving dataset: " + dataset?.Name);

            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            using (var writer = new JsonTextWriter(new StreamWriter(outStream, _uf8Encoding))) { //TODO: Set encoding via options
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

                _logger?.LogDebug("Finish saving dataset: " + dataset?.Name);
            }
        }

        /// <summary>
        /// Writes the table schema.
        /// </summary>
        /// <param name="writer">A JSON writer.</param>
        /// <param name="dataReader">A data reader that allows to get some meta-information of the dataset.</param>
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
