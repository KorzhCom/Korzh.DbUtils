using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Export
{
    public class XmlDatasetExporter: IDatasetExporter
    {
        private readonly ILogger _logger = null;

        public XmlDatasetExporter()
        {
        }

        public string FileExtension => "xml";

        public void ExportDataset(IDataReader dataReader, Stream outStream, string datasetName = null)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++) {
                columns[i] = dataReader.GetName(i);
            }

            using (var writer = new XmlTextWriter(outStream, Encoding.UTF8)) { ///TODO: Set the encoding somewhere else
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();
                writer.WriteStartElement("Dataset");
                if (!string.IsNullOrEmpty(datasetName)) {
                    writer.WriteAttributeString("name", datasetName);
                }

                writer.WriteStartElement("Schema");
                WriteDatasetSchema(writer, dataReader);
                writer.WriteEndElement();

                writer.WriteStartElement("Data");
                while (dataReader.Read()) {
                    _logger?.LogDebug("Start writting row.");
                    writer.WriteStartElement("Row");
                    for (var i = 0; i < dataReader.FieldCount; i++) {
                        var column = columns[i];
                        var value = dataReader.GetValue(i);
                        if (value.GetType() != typeof(DBNull)) {
                            writer.WriteStartElement("Col");
                            writer.WriteAttributeString("n", column);
                            writer.WriteValue(value);
                            writer.WriteEndElement();
                        }

                        _logger?.LogDebug($"Column={column}; Value={value}");
                    }
                    writer.WriteEndElement();
                    _logger?.LogDebug("Finish writting row.");
                }

                writer.WriteEndElement(); //Data
                writer.WriteEndElement(); //Dataset
                writer.WriteEndDocument();
                _logger?.LogInformation("Finish saving dataset");
            }
        }

        protected virtual void WriteDatasetSchema(XmlTextWriter writer, IDataReader dataReader)
        {
            writer.WriteStartElement("Columns");
            var schema = dataReader.GetSchemaTable();
            foreach (DataRow row in schema.Rows) {
                writer.WriteStartElement("Col");
                writer.WriteAttributeString("name", row["ColumnName"].ToString());
                writer.WriteAttributeString("type", row["DataType"].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
