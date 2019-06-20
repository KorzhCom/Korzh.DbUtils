using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using dbexport.DbSavers;

namespace dbexport.DbExporters
{
    internal class MsSqlServerExporter : DbExporterBase
    {
        public MsSqlServerExporter(string connectionString, IDbSaver saver) : base(connectionString, saver)
        {
            DbConnection = new SqlConnection(connectionString);
        }

        protected override IReadOnlyCollection<string> GetTables()
        {
            DataTable schemaTable = DbConnection.GetSchema(SqlClientMetaDataCollectionNames.Tables);
            var tables = new List<string>();

            foreach (DataRow row in schemaTable.Rows)
            {
                string tableType = (string)row["TABLE_TYPE"];
                string tableName = (string)row["TABLE_NAME"];
                if (tableType == "BASE TABLE")
                    tables.Add(tableName);
            }

            return tables.AsReadOnly();
        }

        protected override IDataReader ReadTable(string tableName)
        {
            var sql = "SELECT * FROM " + tableName;

            IDataReader reader = null;
            using (var command = DbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                reader = command.ExecuteReader();
            }

            return reader;          
        }
    }
}
