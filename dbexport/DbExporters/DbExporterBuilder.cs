using System;
using System.Collections.Generic;
using System.Text;

using dbexport.Savers;

namespace dbexport.DbExporters
{
    internal enum DbType {
        Unknown,
        MsSqlServer,
        MySql
    }

    internal class DbExportBuilderException : Exception
    {
        public DbExportBuilderException(string message) : base(message)
        {

        }
    }

    internal class DbExporterBuilder
    {
        private Type _dbExporterType;
        private string _connectionString;
        private IDbSaver _saver;

        public DbExporterBuilder SetConnectionString(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            return this;
        }

        public DbExporterBuilder UseDbExporter(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.MsSqlServer:
                    _dbExporterType = typeof(MsSqlServerExporter);
                    break;
                case DbType.MySql:
                    _dbExporterType = typeof(MySqlExporter);
                    break;
                default:
                    throw new DbExportBuilderException("Unknown Database type");
            }

            return this;

        }

        public DbExporterBuilder UseDbExporter<T>() where T : DbExporterBase
        {
            _dbExporterType = typeof(T);
            return this;
        }

        public DbExporterBuilder UseFileDbSaver(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (fileName.EndsWith(".json"))
            {
                _saver = new JsonFileDbSaver(fileName);
            }
            else if (fileName.EndsWith(".xml"))
            {
                _saver = new XmlFileDbSaver(fileName);
            }
            else {
                fileName += ".xml";
                _saver = new XmlFileDbSaver("filaName");
            }

            return this;
        }

        public DbExporterBuilder UseXmlFileDbSaver(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (!fileName.EndsWith(".xml"))
                fileName += ".xml";

            _saver = new XmlFileDbSaver(fileName);
            return this;
        }

        public DbExporterBuilder UseJsonFileDbSaver(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            _saver = new JsonFileDbSaver(fileName);
            return this;
        }

        public DbExporterBuilder UseDbSaver(IDbSaver saver)
        {
            _saver = saver;
            return this;
        }

        public IDbExporter Build()
        {
            if (_connectionString == null)
            {
                throw new DbExportBuilderException("Connection string is not defined");
            }

            if (_dbExporterType == null)
            {
                throw new DbExportBuilderException("DbExporter is not defined");
            }

            if (_saver == null)
            {
                _saver = new XmlFileDbSaver("result.xml");
            }

            return (IDbExporter)
                Activator.CreateInstance(_dbExporterType, _connectionString, _saver);
        }

    }
}
