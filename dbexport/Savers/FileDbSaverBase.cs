using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace dbexport.Savers
{
    public abstract class FileDbSaverBase: IDbSaver
    {
        private readonly string _fileName;

        protected StreamWriter FileStream;

        public FileDbSaverBase(string fileName)
        {
            _fileName = fileName;
        }

        public virtual void Start()
        {
            FileStream = File.CreateText(_fileName);
        }

        public abstract void StartSaveTable(string tableName);

        public abstract void SaveTableData(IDataReader dataReader);

        public abstract void EndSaveTable();

        public virtual void End()
        {
            FileStream.Close();
        }
    }
}
