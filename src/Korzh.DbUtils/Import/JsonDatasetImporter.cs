using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Korzh.DbUtils.Import
{
    public class JsonDatasetImporter : IDatasetImporter
    {
        private JsonTextReader _jsonReader;
        private bool _isEndOfRecords = false;

        public void StartImport(Stream datasetStream)
        {
            _jsonReader = new JsonTextReader(new StreamReader(datasetStream));
            _jsonReader.Read();
            if (_jsonReader.TokenType != JsonToken.StartObject) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }
            _isEndOfRecords = false;

            if (!ReadToProperty("schema")) {
                _isEndOfRecords = true;
                throw new DatasetImporterException($"Wrong file format. No 'schema' property");
            }
            _jsonReader.Read();
            ReadSchema();
            if (!ReadToProperty("data")) {
                _isEndOfRecords = true;
                throw new DatasetImporterException($"Wrong file format. No 'data' property");
            }
            _jsonReader.Read();
            if (_jsonReader.TokenType != JsonToken.StartArray) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }
            _jsonReader.Read(); //read first object start
            if (!_jsonReader.Read() || _jsonReader.TokenType != JsonToken.StartObject) {
                _isEndOfRecords = true;
            }
        }

        private bool ReadToProperty(string propName)
        {
            while (_jsonReader.Read()) {
                if (_jsonReader.TokenType == JsonToken.PropertyName && _jsonReader.Value.ToString() == propName) {
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
            return !_isEndOfRecords;
        }

        public IDataRecord NextRecord()
        {
            var record = new DataRecord();

            ReadRecordFields(record);

            if (!_jsonReader.Read() || _jsonReader.TokenType != JsonToken.StartObject) {
                _isEndOfRecords = true;
            }
            return record;
        }

        protected virtual void ReadRecordFields(DataRecord record)
        {
            while (_jsonReader.Read()) {
                if (_jsonReader.TokenType == JsonToken.PropertyName) {
                    var fieldName = _jsonReader.Value.ToString();
                    _jsonReader.Read();
                    ReadOneRecordField(record, fieldName, _jsonReader.Value);
                }
                else if (_jsonReader.TokenType == JsonToken.EndObject) {
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
            _jsonReader.Close();
        }
    }
}
