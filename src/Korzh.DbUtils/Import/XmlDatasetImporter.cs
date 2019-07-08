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

        public void StartImport(Stream datasetStream)
        {
            _xmlReader = new XmlTextReader(new StreamReader(datasetStream));
            _xmlReader.Read();
            if (_xmlReader.NodeType != XmlNodeType.Element) {
                throw new DatasetImporterException($"Wrong file format at {_xmlReader.LineNumber}:{_xmlReader.LinePosition}");
            }
            _isEndOfData = false;

            if (!ReadToElement("Schema")) {
                _isEndOfData = true;
                throw new DatasetImporterException($"Wrong file format. No 'Schema' element");
            }
            _xmlReader.Read();
            ReadSchema();
            if (!ReadToElement("Data")) {
                _isEndOfData = true;
                throw new DatasetImporterException($"Wrong file format. No 'Data' element");
            }

            if (!ReadToElement("Row")) {
                _isEndOfData = true;
            }
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

                    ReadOneRecordField(record, fieldName, _xmlReader.ReadElementContentAsObject());
                }
                else if (_xmlReader.NodeType == XmlNodeType.EndElement) {
                    break;
                }
            }
        }

        protected virtual void ReadOneRecordField(DataRecord record, string fieldName, object value)
        {
            record.SetProperty(fieldName, value);
        }

        public void FinishImport()
        {
            _xmlReader.Close();
        }
    }
}
