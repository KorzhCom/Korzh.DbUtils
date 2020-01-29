using System;
using System.Collections.Generic;
using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Sqlite
{
    /// <summary>
    /// An implementation of <see cref="BaseDbBridge "/> for Sqlite
    /// Implements the <see cref="BaseDbBridge" />
    /// </summary>
    /// <seealso cref="BaseDbBridge" />
    public class SqliteBridge : BaseDbBridge
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqliteBridge(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SqliteBridge(string connectionString, ILoggerFactory loggerFactory)
            : base(connectionString, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SqliteBridge(SqliteConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SqliteBridge(SqliteConnection connection, ILoggerFactory loggerFactory)
            : base(connection, loggerFactory)
        {
        }

        /// <summary>
        /// Creates an SqliteConnection object.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        /// <summary>
        /// Extracts the list of tables for the current DB and saves them to the list object passed in the parameter.
        /// </summary>
        /// <param name="datasets">The list of datasets (tables) to fill.</param>
        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            using (var command = Connection.CreateCommand()) {

                command.CommandText = "select name, type from sqlite_master where type = 'table'";

                using (var dataReader = command.ExecuteReader()) {

                    while (dataReader.Read()) {
                        var tableName = dataReader["name"].ToString();
                        
                        datasets.Add(new DatasetInfo(tableName, ""));
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the list of columns for the current table and saves them to the list object passed in the parameter.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The list which will be filled with the columns from the specified table.</param>
        protected override void ExtractColumnList(string tableSchema, string tableName, IList<ColumnInfo> columns)
        {
            using (var command = Connection.CreateCommand()) {
                command.CommandText = string.Format("PRAGMA table_info('{0}')", tableName);

                using (var dataReader = command.ExecuteReader()) {
                    while (dataReader.Read()) {
                        var columnName = dataReader["name"].ToString();
                        var dbTypeName = dataReader["type"].ToString();
                        var isPK = dataReader["pk"].ToString() == "1";

                        ColumnInfo column = new ColumnInfo(columnName, SqliteDbTypeToClrType(dbTypeName), isPK);

                        columns.Add(column);
                    }
                }
            }
        }

        //converts Sqlite db types to CLR types
        private Type SqliteDbTypeToClrType(string type)
        {
            switch (type)
            {
                case "bit":
                    return typeof(bool);

                case "tinyint":
                case "smallint":
                    return typeof(short);

                case "int":
                case "integer":
                    return typeof(int);

                case "bigint":
                    return typeof(long);

                case "real":
                case "float":
                case "numeric":
                    return typeof(float);

                case "decimal":
                    return typeof(decimal);

                case "binary":
                case "varbinary":
                    return typeof(byte[]);

                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return typeof(DateTime);

                case "time":
                case "timestamp":
                    return typeof(TimeSpan);

                case "datetimeoffset":
                    return typeof(DateTimeOffset);

                default:
                    return typeof(string);
            }
        }

        protected override void TurnOffAutoIncrement()
        {
            //NOTHING TO DO
        }

        protected override void TurnOffConstraints()
        {
            using (var command = Connection.CreateCommand()) {
                command.CommandText = "PRAGMA foreign_keys = OFF;";
                command.ExecuteNonQuery();
            }
        }

        protected override void TurnOnAutoIncrement()
        {
            //NOTHING TO DO
        }

        protected override void TurnOnConstraints()
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();
            }
        }
    }
}
