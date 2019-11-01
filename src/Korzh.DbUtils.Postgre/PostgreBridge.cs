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
    /// An implementation of <see cref="BaseDbBridge "/> for MySQL
    /// Implements the <see cref="BaseDbBridge" />
    /// </summary>
    /// <seealso cref="BaseDbBridge" />
    public class PostgreBridge : BaseDbBridge
    {
        public PostgreBridge(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public PostgreBridge(NpgsqlConnection connection) : base(connection)
        {
        }

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
        /// Creates the connection.
        /// This is an abstract method which must be implemented in derived classes
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

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

        protected override string Quote1 => "\"";
               
        protected override string Quote2 => "\"";

        protected override void TurnOffAutoIncrement()
        {
            // May be need query text like this -- SET IDENTITY_INSERT {table} OFF;  -- work without his
        }

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

        protected override void TurnOnAutoIncrement()
        {
            // May be need query text like this -- SET IDENTITY_INSERT {table} ON;  -- work without his
        }

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
