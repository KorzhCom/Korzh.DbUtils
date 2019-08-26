using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Export
{
    /// <summary>
    /// Represents an implmentation of <see cref="Korzh.DbUtils.IDatasetExporter" /> which stores the conten of some dataset to XML format.
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDatasetExporter" />
    public class XmlDatasetExporter: IDatasetExporter
    {
        private readonly ILogger _logger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDatasetExporter"/> class.
        /// </summary>
        public XmlDatasetExporter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDatasetExporter"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public XmlDatasetExporter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Gets the default file extension for the current exporting format.
        /// </summary>
        /// <value>"xml"</value>
        public string FileExtension => "xml";

        private static Encoding Encoding = new UTF8Encoding(false);

        /// <summary>
        /// Exports the dataset's content to XML
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

            for (int i = 0; i < dataReader.FieldCount; i++) {
                columns[i] = dataReader.GetName(i);
            }

            using (var writer = new XmlTextWriter(outStream, Encoding)) { 
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();
                writer.WriteStartElement("Dataset");
                if (!string.IsNullOrEmpty(dataset?.Name)) {
                    writer.WriteAttributeString("name", dataset.Name);
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

                _logger?.LogDebug("Finish saving dataset: " + dataset?.Name);
            }
        }

        /// <summary>
        /// Writes the table schema.
        /// </summary>
        /// <param name="writer">An XML writer.</param>
        /// <param name="dataReader">A data reader that allows to get some meta-information of the dataset.</param>
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
