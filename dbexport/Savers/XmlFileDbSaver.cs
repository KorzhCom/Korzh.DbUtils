using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

namespace dbexport.Savers
{
    internal class XmlFileDbSaver: FileDbSaverBase
    {

        private XmlTextWriter _writer;

        public XmlFileDbSaver(string fileName) : base(fileName)
        {

        }

        public override void Start()
        {
            base.Start();

            _writer = new XmlTextWriter(FileStream);
            _writer.Formatting = Formatting.Indented;

            _writer.WriteStartDocument();
            _writer.WriteStartElement("Root");
        }


        public override void StartSaveTable(string tableName)
        {
            _writer.WriteStartElement(tableName);
        }

        public override void SaveTableData(IDataReader dataReader)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            while (dataReader.Read())
            {
                _writer.WriteStartElement("Row");
                foreach (var column in columns)
                {
                    _writer.WriteStartElement(column);
                    var value = dataReader.GetValue(dataReader.GetOrdinal(column));
                    if (value.GetType() != typeof(DBNull))
                    {
                        _writer.WriteValue(dataReader.GetValue(dataReader.GetOrdinal(column)));
                    }

                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
            }
        }

       
        public override void EndSaveTable()
        {
            _writer.WriteEndElement();
        }

        public override void End()
        {
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Close();

            base.End();
        }
    }
}
