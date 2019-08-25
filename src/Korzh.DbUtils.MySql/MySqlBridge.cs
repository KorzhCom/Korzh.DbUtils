using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

namespace Korzh.DbUtils.MySql
{
    /// <summary>
    /// An implementation of <see cref="BaseDbBridge "/> for MySQL
    /// Implements the <see cref="Korzh.DbUtils.BaseDbBridge" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.BaseDbBridge" />
    public class MySqlBridge : BaseDbBridge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySqlBridge(string connectionString) 
            : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MySqlBridge(string connectionString, ILoggerFactory loggerFactory) 
            : base(connectionString, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MySqlBridge(MySqlConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MySqlBridge(MySqlConnection connection, ILoggerFactory loggerFactory) 
            : base(connection, loggerFactory)
        {
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }


        /// <summary>
        /// Extracts the list of columns for the current table and saves them to columns list passed in the parameter.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The columns list.</param>
        protected override void ExtractColumnsList(string tableSchema, string tableName, IList<ColumnInfo> columns)
        {
            string[] restrictions = new string[3];
            restrictions[2] = tableName;

            DataTable schemaTable = Connection.GetSchema("Columns", restrictions);
            foreach (DataRow row in schemaTable.Rows) {
                var columnName = (string)row["column_name"];
                var type = (string)row["data_type"];
                ColumnInfo column = new ColumnInfo(columnName, SQLTypeToCLRType(type));
                columns.Add(column);
            }
        }

        private Type SQLTypeToCLRType(string type)
        {
            switch (type)
            {
                case "tinyint":
                case "smallint":
                    return typeof(short);
                case "int":
                    return typeof(int);
                case "bigint":
                case "mediumint":
                case "set":
                    return typeof(long);
                case "float":
                    return typeof(float);
                case "double":
                case "real":
                    return typeof(double);
                case "decimal":
                case "numeric":
                    return typeof(decimal);
                case "tinyblob":
                case "blob":
                case "mediumblob":
                case "longblob":
                    return typeof(byte[]);
                case "date":
                case "datetime":
                    return typeof(DateTime);
                case "time":
                case "timestamp":
                    return typeof(TimeSpan);
                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// Extracts the list of tables for the current DB and saves them to datasets list passed in the parameter.
        /// </summary>
        /// <param name="datasets">The list of datasets (tables) to fill.</param>
        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            using (var dataReader = GetDataReaderForSql("SHOW TABLES")) {
                while (dataReader.Read()) {
                    string tableName = dataReader.GetString(0);
                    datasets.Add(new DatasetInfo(tableName, ""));
                }
            }
        }

        /// <summary>
        /// Gets the opening quote for SQL identifirs (table/field names, etc).
        /// </summary>
        /// <value>The symbol(s) which represents the opening quote. The default value is '['</value>
        protected override string Quote1 => "`";

        /// <summary>
        /// Gets the closing quote for SQL identifirs (table/field names, etc).
        /// </summary>
        /// <value>The symbol(s) which represents the closing quote. The default value is ']'</value>
        protected override string Quote2 => "`";

        /// <summary>
        /// Adds the parameters to the DB command object.
        /// </summary>
        /// <param name="command">The DB command.</param>
        /// <param name="record">The record. Each field in this record will be added a parameter.</param>
        protected override void AddParameters(IDbCommand command, IDataRecord record)
        {
            for (int i = 0; i < record.FieldCount; i++) {
                var parameter = new MySqlParameter(ToParameterName(record.GetName(i)), record.GetValue(i)) {
                    Direction = ParameterDirection.Input,
                    MySqlDbType = record.GetFieldType(i).ToMySqlDbType()
                };

                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Sends an SQL command which turns off the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOffAutoIncrement()
        {
            // NOTHING TO DO: THERE IS NO PROBLEM WITH AUTO_INCREMENT FIELDS
        }

        /// <summary>
        /// Sends an SQL command which turns on the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOnAutoIncrement()
        {
            // NOTHING TO DO: THERE IS NO PROBLEM WITH AUTO_INCREMENT FIELDS
        }

        /// <summary>
        /// Sends an SQL command which turns off the constraints for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOffConstraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"SET FOREIGN_KEY_CHECKS = 0;";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Sends an SQL command which turns the constraints on for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected override void TurnOnConstraints()
        {
            using (var command = GetConnection().CreateCommand()) {
                command.CommandText = $"SET FOREIGN_KEY_CHECKS = 1;";
                command.CommandType = CommandType.Text;

                Logger?.LogInformation(command.CommandText);

                command.ExecuteNonQuery();
            }
        }
    }
}
