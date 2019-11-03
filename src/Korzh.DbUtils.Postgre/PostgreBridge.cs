using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Microsoft.Extensions.Logging;

using Npgsql;

namespace Korzh.DbUtils.Postgre
{
    /// <summary>
    /// An implementation of <see cref="BaseDbBridge "/> for PostgreSql
    /// Implements the <see cref="BaseDbBridge" />
    /// </summary>
    /// <seealso cref="BaseDbBridge" />
    public class PostgreBridge : BaseDbBridge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgreBridge(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreBridge"/> class.
        /// </summary>
        /// <param name="connection">An instanc eof Npgsql connection.</param>
        public PostgreBridge(NpgsqlConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public PostgreBridge(string connectionString, ILoggerFactory loggerFactory) : base(connectionString, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public PostgreBridge(NpgsqlConnection connection, ILoggerFactory loggerFactory) : base(connection, loggerFactory)
        {
        }

        /// <summary>
        /// Creates an NpgsqlConnection object.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Extracts the list of columns for the current table and saves them to the list object passed in the parameter.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The list which will be filled with the columns from the specified table.</param>
        protected override void ExtractColumnList(string tableSchema, string tableName, IList<ColumnInfo> columns)
        {
            string[] restrictions = new string[3];
            restrictions[2] = tableName;

            DataTable schemaTable = Connection.GetSchema("Columns", restrictions);
            
            foreach (DataRow row in schemaTable.Rows){
                var columnName = (string)row["column_name"];
                var dbTypeName = (string)row["data_type"];

                ColumnInfo column = new ColumnInfo(columnName, PostgreDbTypeToClrType(dbTypeName));
                column.IsTimestamp = dbTypeName == "timestamp" || dbTypeName == "timestamptz" || dbTypeName == "timetz";
                
                columns.Add(column);
            }            
        }

        //converts PosgreSql types to CLR types
        private Type PostgreDbTypeToClrType(string type)
        {
            switch (type)
            {
                case "boolean":
                case "bool":
                case "bit(1)":
                    return typeof(bool);

                case "smallint":
                case "int2":
                    return typeof(short);

                case "integer":
                case "int4":
                    return typeof(int);

                case "bigint":
                case "int8":
                    return typeof(long);

                case "real":
                case "float8":
                    return typeof(float);

                case "numeric":
                case "money":
                    return typeof(decimal);

                case "bit(n)":
                case "bitvarying":
                case "varbit":
                    return typeof(BitArray);

                case "uuid":
                    return typeof(Guid);

                case "bytea": 
                    return typeof(byte[]);

                case "date":
                case "timestamp":
                case "timestamptz":
                case "timetz":
                    return typeof(DateTime);

                case "interval":
                case "time":
                    return typeof(TimeSpan);

                case "time with time zone":
                    return typeof(DateTimeOffset);

                case "record":
                    return typeof(object[]);

                case "char":
                case "(internal) char":
                    return typeof(char);

                case "cid":
                case "xid":
                case "oid":
                    return typeof(uint);

                case "varchar":
                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// Extracts the list of tables for the current DB and saves them to the list object passed in the parameter.
        /// </summary>
        /// <param name="datasets">The list of datasets (tables) to fill.</param>
        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            DataTable schemaTable = Connection.GetSchema("Tables");

            foreach (DataRow row in schemaTable.Rows){
                string tableType = (string)row["TABLE_TYPE"];
                string tableName = (string)row["TABLE_NAME"];
                string tableSchema = (string)row["TABLE_SCHEMA"];
                if (tableType == "BASE TABLE"){
                    datasets.Add(new DatasetInfo(tableName, tableSchema));
                }
            }
        }

        /// <summary>
        /// Gets the opening quote for SQL identifirs (table/field names, etc).
        /// </summary>
        /// <value>The symbol(s) which represents the opening quote. Returns '"'</value>
        protected override string Quote1 => "\"";

        /// <summary>
        /// Gets the closing quote for SQL identifirs (table/field names, etc).
        /// </summary>
        /// <value>The symbol(s) which represents the closing quote. Returns '"'</value>
        protected override string Quote2 => "\"";

        /// <summary>
        /// Sends an SQL command which turns off the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Not used in this class
        /// </summary>
        protected override void TurnOffAutoIncrement()
        {
            // May be need query text like this -- SET IDENTITY_INSERT {table} OFF;  -- work without his
        }

        /// <summary>
        /// Sends an SQL command which turns off the constraints for the current table.
        /// </summary>
        protected override void TurnOffConstraints()
        {
            if (CurrentSeedingTable == null)
                return;
            
            using (var command = GetConnection().CreateCommand()){
                command.CommandText = $"ALTER TABLE {GetTableFullName(CurrentSeedingTable)} disable trigger all;";
                command.CommandType = CommandType.Text;

                Logger?.LogDebug(command.CommandText);

                command.ExecuteNonQuery();
            }            
        }

        /// <summary>
        /// Sends an SQL command which turns on the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Not used in this class
        /// </summary>
        protected override void TurnOnAutoIncrement()
        {
            // May be need query text like this -- SET IDENTITY_INSERT {table} ON;  -- work without his
        }

        /// <summary>
        /// Sends an SQL command which turns the constraints on for the current table.
        /// </summary>
        protected override void TurnOnConstraints()
        {
            if (CurrentSeedingTable == null)
                return;

            using (var command = GetConnection().CreateCommand()){
                command.CommandText = $"ALTER TABLE {GetTableFullName(CurrentSeedingTable)} enable trigger all;";
                command.CommandType = CommandType.Text;

                Logger?.LogDebug(command.CommandText);

                command.ExecuteNonQuery();
            }
        }
    }
}
