using System;
using System.Data;
using System.IO;
using System.Xml;

namespace Korzh.DbUtils.Import
{
    public class XmlDatasetImporter : IDatasetImporter
    {
        private XmlTextReader _xmlReader;
        private bool _isEndOfData = false;

        public string FileExtension => "xml";

        private DatasetInfo _datasetInfo;

        public DatasetInfo StartImport(Stream datasetStream)
        {
            _isEndOfData = true;

            _xmlReader = new XmlTextReader(new StreamReader(datasetStream));
            if (!ReadToElement("Dataset")) {
                throw new DatasetImporterException($"Wrong file format. No 'Dataset' element");
            }

            var datasetInfo = new DatasetInfo(_xmlReader.GetAttribute("name"));
            _datasetInfo = datasetInfo;

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

        protected virtual void ReadSchema()
        {
            if (ReadToElement("Columns")) {
                while (_xmlReader.Read()
                    && _xmlReader.NodeType == XmlNodeType.Element) {

                    _datasetInfo.AddColumn(new ColumnInfo(_xmlReader.GetAttribute("name"), _xmlReader.GetAttribute("type")));
                }
            }
        }

        public bool HasRecords()
        {
            return !_isEndOfData;
        }

        public IDataRecord NextRecord()
        {
            var record = new DataRecord();

            ReadRecordFields(record);

            if (!ReadToElement("Row")) {
                _isEndOfData = true;
            }
            return record;
        }

        protected virtual void ReadRecordFields(DataRecord record)
        {
            while (_xmlReader.Read()) {
                if (_xmlReader.NodeType == XmlNodeType.Element) {
                    var fieldName = _xmlReader.GetAttribute("n");
                    if (fieldName == null) {
                        throw new DatasetImporterException($"Wrong file format. No 'n' attribute in a row");
                    }

                    ReadOneRecordField(record, fieldName, _xmlReader.ReadElementContentAsString());
                }
                else if (_xmlReader.NodeType == XmlNodeType.EndElement) {
                    break;
                }
            }
        }

        protected virtual void ReadOneRecordField(DataRecord record, string fieldName, string value)
        {
            record.SetProperty(fieldName, _datasetInfo.Columns[fieldName].DataType, value);
        }

        public void FinishImport()
        {
            _xmlReader.Close();
            _datasetInfo = null;
        }
    }
}
