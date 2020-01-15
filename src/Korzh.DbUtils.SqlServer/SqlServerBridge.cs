using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.SqlServer
{
    /// <summary>
    /// An implementation of <see cref="BaseDbBridge "/> for MySQL
    /// Implements the <see cref="Korzh.DbUtils.BaseDbBridge" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.BaseDbBridge" />
    public class SqlServerBridge : BaseDbBridge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlServerBridge(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SqlServerBridge(string connectionString, ILoggerFactory loggerFactory) 
            : base(connectionString, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SqlServerBridge(SqlConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SqlServerBridge(SqlConnection connection, ILoggerFactory loggerFactory) 
            : base(connection, loggerFactory)
        {
        }

        /// <summary>
        /// Creates the connection.
        /// This is an abstract method which must be implemented in derived classes
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Extracts the list of columns for the current table and saves them to columns list passed in the parameter.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The columns list.</param>
        protected override void ExtractColumnList(string tableSchema, string tableName, IList<ColumnInfo> columns)
        {
            string[] restrictions = new string[4];
            restrictions[2] = tableName;

            //Get key-fields from table
            var sqlForKeys = @"SELECT CCU.COLUMN_NAME
                               FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                               JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ON
                               TC.CONSTRAINT_CATALOG = CCU.CONSTRAINT_CATALOG AND
                               TC.CONSTRAINT_SCHEMA = CCU.CONSTRAINT_SCHEMA AND
                               TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME AND
                               TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND
                               TC.TABLE_SCHEMA = '" + tableSchema + @"' 
                               AND TC.TABLE_NAME = '" + tableName + "';";

            var primaryKeys = new List<string>();
            using (var command = this.Connection.CreateCommand()) {
                command.CommandText = sqlForKeys;
                command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        primaryKeys.Add(reader[0].ToString());
                    }
                }
            }

            DataTable schemaTable = Connection.GetSchema(SqlClientMetaDataCollectionNames.Columns, restrictions);
            foreach (DataRow row in schemaTable.Rows) {
                var columnName = row["COLUMN_NAME"] as string;
                var type = row["DATA_TYPE"] as string;
                var isPK = primaryKeys.Contains(columnName);
                ColumnInfo column = new ColumnInfo(columnName, SqlTypeToClrType(type), isPK);
                column.IsTimestamp = type == "rowversion" || type == "timestamp";
                columns.Add(column);
            }
        }

        private Type SqlTypeToClrType(string type)
        {
            switch(type) {
                case "bigint":
                    return typeof(long);
                case "numeric":
                    return typeof(float);
                case "bit":
                    return typeof(bool);
                case "int":
                    return typeof(int);
                case "smallint":
                    return typeof(short);
                case "smallmoney":
                case "money":
                case "decimal":
                    return typeof(decimal);
                case "float":
                    return typeof(float);
                case "real":
                    return typeof(double);
                case "smalldatetime":
                case "datetime":
                    return typeof(DateTime);
                case "datetime2":
                case "datetimeoffset":
                    return typeof(DateTimeOffset);
                case "time":
                    return typeof(TimeSpan);
                case "binary":
                case "varbinary":
                case "image":
                    return typeof(byte[]);
                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// Gets the list of tables for the current DB and saves them to datasets list passed in the parameter.
        /// </summary>
        /// <param name="datasets">The list of datasets (tables) to fill.</param>
        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            DataTable schemaTable = Connection.GetSchema(SqlClientMetaDataCollectionNames.Tables);

            foreach (DataRow row in schemaTable.Rows) {
                string tableType = (string)row["TABLE_TYPE"];
                string tableName = (string)row["TABLE_NAME"];
                string tableSchema = (string)row["TABLE_SCHEMA"];
                if (tableType == "BASE TABLE") {
                    datasets.Add(new DatasetInfo(tableName, tableSchema));
                }
            }
        }

        /// <summary>
        /// Sends an SQL command which turns off the constraints for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOffConstraints()
        {
            if (CurrentTable != null) {
                using (var command = GetConnection().CreateCommand()) {
                    command.CommandText = $"ALTER TABLE {GetFormattedTableName(CurrentTable)} NOCHECK CONSTRAINT all";
                    command.CommandType = CommandType.Text;

                    Logger?.LogDebug(command.CommandText);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Sends an SQL command which turns the constraints on for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOnConstraints()
        {
            if (CurrentTable != null) {
                using (var command = GetConnection().CreateCommand()) {
                    command.CommandText = $"ALTER TABLE {GetFormattedTableName(CurrentTable)} CHECK CONSTRAINT all";
                    command.CommandType = CommandType.Text;

                    Logger?.LogDebug(command.CommandText);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Sends an SQL command which turns off the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOffAutoIncrement()
        {
            if (CurrentTable != null) {
                using (var command = GetConnection().CreateCommand()) {
                    command.CommandText = $"IF EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = object_id('{GetFormattedTableName(CurrentTable)}') AND c.is_identity =1) begin SET IDENTITY_INSERT {GetFormattedTableName(CurrentTable)} ON end";
                    command.CommandType = CommandType.Text;

                    Logger?.LogDebug(command.CommandText);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Sends an SQL command which turns on the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOnAutoIncrement()
        {
            if (CurrentTable != null) {
                using (var command = GetConnection().CreateCommand()) {
                    command.CommandText = $"IF EXISTS (SELECT 1 from sys.columns c WHERE c.object_id = object_id('{GetFormattedTableName(CurrentTable)}') AND c.is_identity = 1) begin SET IDENTITY_INSERT {GetFormattedTableName(CurrentTable)} OFF end";
                    command.CommandType = CommandType.Text;

                    Logger?.LogDebug(command.CommandText);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
