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
        private bool _isEndOfData = false;

        public string FileExtension => "json";


        private DatasetInfo _datasetInfo = null;

        public DatasetInfo StartImport(Stream datasetStream)
        {

            _jsonReader = new JsonTextReader(new StreamReader(datasetStream));
            _jsonReader.Read();
            if (_jsonReader.TokenType != JsonToken.StartObject) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }
            _isEndOfData = false;

            if (!ReadToProperty("name")) {
                _isEndOfData = true;
                throw new DatasetImporterException($"Wrong file format. No 'schema' property");
            }

            var datasetInfo = new DatasetInfo(_jsonReader.ReadAsString());
            _datasetInfo = datasetInfo;

            if (!ReadToProperty("schema")) {
                _isEndOfData = true;
                throw new DatasetImporterException($"Wrong file format. No 'schema' property");
            }
            _jsonReader.Read();
            ReadSchema();
            if (!ReadToProperty("data")) {
                _isEndOfData = true;
                throw new DatasetImporterException($"Wrong file format. No 'data' property");
            }
            _jsonReader.Read();
            if (_jsonReader.TokenType != JsonToken.StartArray) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }
            
            //read first object start
            if (!_jsonReader.Read() || _jsonReader.TokenType != JsonToken.StartObject) {
                _isEndOfData = true;
            }

            return datasetInfo;
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
            if (_jsonReader.TokenType != JsonToken.StartObject) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }

            if (ReadToProperty("columns"))  {

                _jsonReader.Read();
                if (_jsonReader.TokenType != JsonToken.StartArray) {
                    throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
                }

                while (_jsonReader.Read()
                    && _jsonReader.TokenType != JsonToken.EndArray) {

                    if (_jsonReader.TokenType != JsonToken.StartObject) {
                        throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
                    }

                    string name = null;
                    string type = null;

                    while (_jsonReader.Read()
                        && _jsonReader.TokenType != JsonToken.EndObject)
                    {

                        var propName = _jsonReader.Value.ToString();
                        if (propName == "name") {
                            name = _jsonReader.ReadAsString();
                        }
                        else if (propName == "type") {
                            type = _jsonReader.ReadAsString();
                        }
                        else {
                            _jsonReader.Skip();
                        }
                    }

                    _datasetInfo.AddColumn(new ColumnInfo(name, type));
                }

                _jsonReader.Read();

            }
            else {
                throw new DatasetImporterException($"Wrong file format. No 'columns' property in 'schema'");
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

            if (!_jsonReader.Read() || _jsonReader.TokenType != JsonToken.StartObject) {
                _isEndOfData = true;
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
            record.SetProperty(fieldName, _datasetInfo.Columns[fieldName].Type, value.ToString());
        }

        public void FinishImport()
        {
            _jsonReader.Close();
            _datasetInfo = null;
        }
    }
}
