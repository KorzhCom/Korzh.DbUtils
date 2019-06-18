using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace dbexport.Savers
{
    internal class JsonFileDbSaver : FileDbSaverBase
    {
        private JsonWriter _writer;

        public JsonFileDbSaver(string fileName): base(fileName)
        {
          
        }

        public override void Start()
        {
            base.Start();

            _writer = new JsonTextWriter(FileStream);
            _writer.Formatting = Formatting.Indented;
            _writer.WriteStartObject();
        }


        public override void StartSaveTable(string entityName)
        {
            _writer.WritePropertyName(entityName);
            _writer.WriteStartArray();
        }

        public override void SaveTableData(IDataReader dataReader)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            while (dataReader.Read())
            {
                _writer.WriteStartObject();
                foreach (var column in columns)
                {
                    _writer.WritePropertyName(column);
                    _writer.WriteValue(dataReader.GetValue(dataReader.GetOrdinal(column)));
                }
                _writer.WriteEndObject();
            }
        }

        public override void EndSaveTable()
        {
            _writer.WriteEndArray();
        }

        public override void End()
        {
            _writer.WriteEnd();
            _writer.Close();

            base.End();
        }
    }
}
