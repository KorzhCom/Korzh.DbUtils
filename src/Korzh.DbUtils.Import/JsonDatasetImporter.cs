using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Korzh.DbUtils.Import
{
    /// <summary>
    /// Represents an importer from JSON format
    /// Implements the <see cref="Korzh.DbUtils.IDatasetImporter" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDatasetImporter" />
    public class JsonDatasetImporter : IDatasetImporter
    {
        private JsonTextReader _jsonReader;
        private bool _isEndOfData = false;

        /// <summary>
        /// Gets the default file extension for the data format processed by this importer (e.g "xml" or "json").
        /// </summary>
        /// <value>"json"</value>
        public string FileExtension => "json";

        private DatasetInfo _datasetInfo = null;

        private ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDatasetImporter"/> class.
        /// </summary>
        public JsonDatasetImporter() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDatasetImporter"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public JsonDatasetImporter(ILoggerFactory loggerFactory)
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
        /// Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}
        /// or
        /// Wrong file format. No 'schema' property
        /// or
        /// Wrong file format. No 'schema' property
        /// or
        /// Wrong file format. No 'data' property
        /// or
        /// Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}
        /// </exception>
        public DatasetInfo StartImport(Stream datasetStream)
        {

            _jsonReader = new JsonTextReader(new StreamReader(datasetStream, Encoding.UTF8));
            _jsonReader.Read();
            if (_jsonReader.TokenType != JsonToken.StartObject) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }
            _isEndOfData = false;

            if (!ReadToProperty("name")) {
                _isEndOfData = true;
                throw new DatasetImporterException($"Wrong file format. No 'schema' property");
            }

            var datasetInfo = new DatasetInfo(_jsonReader.ReadAsString(), "");
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

        /// <summary>
        /// Reads the schema.
        /// </summary>
        /// <exception cref="DatasetImporterException">
        /// Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}
        /// or
        /// Wrong file format. No 'columns' property in 'schema'
        /// </exception>
        protected virtual void ReadSchema()
        {
            if (_jsonReader.TokenType != JsonToken.StartObject) {
                throw new DatasetImporterException($"Wrong file format at {_jsonReader.LineNumber}:{_jsonReader.LinePosition}");
            }

            if (ReadToProperty("columns")) {

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

            if (!_jsonReader.Read() || _jsonReader.TokenType != JsonToken.StartObject) {
                _isEndOfData = true;
            }
            return record;
        }

        /// <summary>
        /// Reads the record fields.
        /// </summary>
        /// <param name="record">The record.</param>
        protected virtual void ReadRecordFields(DataRecord record)
        {
            while (_jsonReader.Read()) {
                if (_jsonReader.TokenType == JsonToken.PropertyName) {
                    var fieldName = _jsonReader.Value.ToString();
                    _jsonReader.Read();
                    ReadOneRecordField(record, fieldName);
                }
                else if (_jsonReader.TokenType == JsonToken.EndObject) {
                    break;
                }
            }
        }

        /// <summary>
        /// Reads one record field by its name
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">Name of the field.</param>
        protected virtual void ReadOneRecordField(DataRecord record, string fieldName)
        {
            var fieldType = _datasetInfo.Columns[fieldName].DataType;
            object value = _jsonReader.ReadAs(fieldType);

            record[fieldName] = value;
        }

        /// <summary>
        /// Finilizing the importing process.
        /// </summary>
        public void FinishImport()
        {
            _jsonReader.Close();
            _datasetInfo = null;
        }
    }

    /// <summary>
    /// Contains some extensions for <see cref="JsonReader"/> 
    /// </summary>
    static class JsonReaderExtensions
    {
        /// <summary>
        /// Reads an object of specified type from JsonReader
        /// </summary>
        /// <param name="jsonReader">An instance of JsonReader.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object ReadAs(this JsonReader jsonReader, Type type)
        {
            var serializer = JsonSerializer.CreateDefault();
            return serializer.Deserialize(jsonReader, type);
        }
    }
}
