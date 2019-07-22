using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Import
{
    /// <summary>
    /// Represents the dataset importer from XML format
    /// Implements the <see cref="Korzh.DbUtils.IDatasetImporter" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDatasetImporter" />
    public class XmlDatasetImporter : IDatasetImporter
    {
        private XmlTextReader _xmlReader;
        private bool _isEndOfData = false;

        /// <summary>
        /// Gets the default file extension for the data format processed by this importer (e.g "xml" or "json").
        /// </summary>
        /// <value>"xml".</value>
        public string FileExtension => "xml";

        private DatasetInfo _datasetInfo;

        private ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDatasetImporter"/> class.
        /// </summary>
        public XmlDatasetImporter() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDatasetImporter"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public XmlDatasetImporter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Starts the importing process.
        /// This function processes the first part of the dataset stream and collect necessary information about the dataset
        /// </summary>
        /// <param name="datasetStream">The dataset stream.</param>
        /// <returns>An instance of the <see cref="T:Korzh.DbUtils.DatasetInfo" /> which contains some basic information about the dataset (table).</returns>
        /// <exception cref="DatasetImporterException">
        /// Wrong file format. No 'Dataset' element
        /// or
        /// Wrong file format. No 'Schema' element
        /// or
        /// Wrong file format. No 'Data' element
        /// </exception>
        public DatasetInfo StartImport(Stream datasetStream)
        {
            _isEndOfData = true;

            _xmlReader = new XmlTextReader(new StreamReader(datasetStream, Encoding.UTF8));
            if (!ReadToElement("Dataset")) {
                throw new DatasetImporterException($"Wrong file format. No 'Dataset' element");
            }

            var datasetInfo = new DatasetInfo(_xmlReader.GetAttribute("name"), ""); // add schema here
            _datasetInfo = datasetInfo;
            _logger?.LogInformation("Start import dataset: " + _datasetInfo?.Name);

            _isEndOfData = false;

            if (!ReadToElement("Schema")) {
                throw new DatasetImporterException($"Wrong file format. No 'Schema' element");
            }

            ReadSchema();

            if (!ReadToElement("Data")) {
                throw new DatasetImporterException($"Wrong file format. No 'Data' element");
            }

            if (ReadToElement("Row")) {
                _isEndOfData = false;
            }


            return datasetInfo;
        }

        private bool ReadToElement(string nodeName)
        {
            while (_xmlReader.Read()) {
                if (_xmlReader.NodeType == XmlNodeType.Element && _xmlReader.LocalName == nodeName) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the dataset schema.
        /// </summary>
        protected virtual void ReadSchema()
        {
            if (ReadToElement("Columns")) {
                while (_xmlReader.Read() 
                    && _xmlReader.NodeType != XmlNodeType.EndElement) {

                    if (_xmlReader.NodeType == XmlNodeType.Element)
                      _datasetInfo.AddColumn(new ColumnInfo(_xmlReader.GetAttribute("name"), _xmlReader.GetAttribute("type")));
                }
            }
        }

        /// <summary>
        /// Determines whether there are more records to process in the input stream.
        /// </summary>
        /// <returns><c>true</c> if this the input stream still has more records for the current dataset; otherwise, <c>false</c>.</returns>
        public bool HasRecords()
        {
            return !_isEndOfData;
        }

        /// <summary>
        /// Processes the next record in the input stream and returns it to the caller.
        /// </summary>
        /// <returns>IDataRecord.</returns>
        public IDataRecord NextRecord()
        {
            var record = new DataRecord();

            ReadRecordFields(record);

            if (!ReadToElement("Row")) {
                _isEndOfData = true;
            }
            return record;
        }

        /// <summary>
        /// Reads the record fields.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="DatasetImporterException">Wrong file format. No 'n' attribute in a row</exception>
        protected virtual void ReadRecordFields(DataRecord record)
        {
            while (_xmlReader.Read()) {
                if (_xmlReader.NodeType == XmlNodeType.Element) {
                    var fieldName = _xmlReader.GetAttribute("n");
                    if (fieldName == null) {
                        throw new DatasetImporterException($"Wrong file format. No 'n' attribute in a row");
                    }

                    ReadOneRecordField(record, fieldName);
                }
                else if (_xmlReader.NodeType == XmlNodeType.EndElement) {
                    break;
                }
            }
        }

        /// <summary>
        /// Reads one field in the specified record by its (field) name
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">Name of the field.</param>
        protected virtual void ReadOneRecordField(DataRecord record, string fieldName)
        {
            var fieldType = _datasetInfo.Columns[fieldName].DataType;
            object value = _xmlReader.ReadElementContentAs(fieldType, null);

            record[fieldName] = value;
        }

        /// <summary>
        /// Finilizing the importing process.
        /// </summary>
        public void FinishImport()
        {
            _xmlReader.Close();
            _logger?.LogInformation("Finish import dataset: " + _datasetInfo?.Name);
            _datasetInfo = null;
        }
    }
}
