using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

using Microsoft.Extensions.Logging;

namespace Korzh.DbInitializer.DbSavers
{
    internal class XmlZipFileDbSaver: ZipFileDbSaver
    {

        private StreamWriter _fileWriter;
        private XmlTextWriter _writer;

        public XmlZipFileDbSaver(string fileName, ILogger logger) : base(fileName, logger)
        {

        }

        public override void StartSaveTable(string tableName)
        {
            Logger?.LogInformation($"Start saving table '{tableName}' to file '{tableName + ".xml"}'");
            ZipArchiveEntry entry = ZipArchive.CreateEntry(tableName + ".xml");
            _fileWriter = new StreamWriter(entry.Open());

            _writer = new XmlTextWriter(_fileWriter);
            _writer.Formatting = Formatting.Indented;

            _writer.WriteStartDocument();
            _writer.WriteStartElement("Root");
        }

        public override void SaveTableData(IDataReader dataReader)
        {
            var columns = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                columns[i] = dataReader.GetName(i);

            while (dataReader.Read())
            {
                Logger?.LogDebug("Start writting row.");
                _writer.WriteStartElement("Row");
                foreach (var column in columns)
                {
   
                    var value = dataReader.GetValue(dataReader.GetOrdinal(column));
                    if (value.GetType() != typeof(DBNull))
                    {
                        _writer.WriteStartElement(column);
                        _writer.WriteValue(dataReader.GetValue(dataReader.GetOrdinal(column)));
                        _writer.WriteEndElement();
                    }

                    Logger?.LogDebug($"Column={column}; Value={value}");
                }
                _writer.WriteEndElement();
                Logger?.LogDebug("Finish writting row.");
            }
        }

       
        public override void EndSaveTable()
        {
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Close();

            _fileWriter.Close();

            Logger?.LogInformation("Finish saving table");
        }
    }
}
