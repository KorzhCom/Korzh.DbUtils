using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils
{
    /// <summary>
    /// A default abstract implementation of the main DB related intefaces 
    /// <see cref="Korzh.DbUtils.IDbReader" /> and <see cref="Korzh.DbUtils.IDbWriter" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDbReader" />
    /// <seealso cref="Korzh.DbUtils.IDbWriter" />
    public abstract class BaseDbBridge : IDbReader, IDbWriter
    {
        /// <summary>
        /// The database connection
        /// </summary>
        protected DbConnection Connection = null;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger = null;

        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected BaseDbBridge(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        protected BaseDbBridge(string connectionString, ILoggerFactory loggerFactory): this(connectionString)
        {
            Logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected BaseDbBridge(DbConnection connection)
        {
            Connection = connection;
            _connectionString = connection.ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        protected BaseDbBridge(DbConnection connection, ILoggerFactory loggerFactory): this(connection)
        {
            Logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Checks the current connections. Creates it (calls <see cref="CreateConnection(string)"/> methods for that) if necessary.
        /// Then this method opens the connection if it's closed now.
        /// </summary>
        protected void CheckConnection()
        {
            if (Connection == null) {
                Connection = CreateConnection(_connectionString);
            }

            if (Connection.State != ConnectionState.Open) {
                Connection.Open();
            }
        }

        /// <summary>
        /// Creates the connection. 
        /// This is an abstract method which must be implemented in derived classes
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected abstract DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection GetConnection()
        {
            CheckConnection();
            return Connection;
        }

        /// <summary>
        /// Executes the SQL statement passed in the parameter and returns a <see cref="IDataReader" /> object with the result of that execution.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <returns>IDataReader.</returns>
        public IDataReader GetDataReaderForSql(string sql)
        {
            var connection = GetConnection();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            Logger?.LogDebug(command.CommandText);

            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }

        /// <summary>
        /// Gets the opening quote for SQL identifirs (table/field names, etc).
        /// </summary>
        /// <value>The symbol(s) which represents the opening quote. The default value is '['</value>
        protected virtual string Quote1 => "[";

        /// <summary>
        /// Gets the closing quote for SQL identifirs (table/field names, etc).
        /// </summary>
        /// <value>The symbol(s) which represents the closing quote. The default value is ']'</value>
        protected virtual string Quote2 => "]";

        /// <summary>
        /// Creates and returns a <see cref="IDataReader" /> object for some table.
        /// This method usually just constructs a correct SQL statement to get the content of some table (like 'SELECT * FROM TableName)
        /// and then calls <see cref="GetDataReaderForSql(string)" /> function.
        /// </summary>
        /// <param name="table">The table represented by <see cref="DatasetInfo" /> structure.</param>
        /// <returns>IDataReader.</returns>
        public IDataReader GetDataReaderForTable(DatasetInfo table)
        {
            CheckConnection();

            var columns = new List<ColumnInfo>();
            ExtractColumnList(table.Schema, table.Name, columns);

            var sql = new StringBuilder();
            sql.Append("SELECT");

            foreach (var column in columns) {
                sql.AppendFormat(" {0}{1}{2},", Quote1, column.Name, Quote2);
            }

            sql.Remove(sql.Length - 1, 1);
            sql.AppendFormat(" FROM {0}", GetTableFullName(table));

            return GetDataReaderForSql(sql.ToString());
        }


        /// <summary>
        /// Extracts the list of columns for the current table and saves them to columns list passed in the parameter.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The columns list.</param>
        protected abstract void ExtractColumnList(string tableSchema, string tableName, IList<ColumnInfo> columns);


        /// <summary>
        /// Gets the list of tables (or other kind of dataset) for the current connection.
        /// This method calls <see cref="ExtractDatasetList"/> function to actually get the list of tables.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;DatasetInfo&gt;.</returns>
        public IReadOnlyCollection<DatasetInfo> GetDatasets()
        {
            CheckConnection();
            var tables = new List<DatasetInfo>();

            ExtractDatasetList(tables);

            return tables.AsReadOnly();
        }


        /// <summary>
        /// Extracts the list of tables for the current DB and saves them to datasets list passed in the parameter.
        /// </summary>
        /// <param name="datasets">The list of datasets (tables) to fill.</param>
        protected abstract void ExtractDatasetList(IList<DatasetInfo> datasets);

        /// <summary>
        /// Inserts a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        /// <exception cref="Korzh.DbUtils.DbBridgeException">Seeding is not stared. Call StartSeeding() before.</exception>
        public void InsertRecord(IDataRecord record)
        {
            if (CurrentTable is null || _insertCommand is null) {
                throw new DbBridgeException("Seeding is not stared. Call StartSeeding() before." );
            }

            CheckConnection();

            FillParameters(_insertCommand, record);

            Logger?.LogDebug(_insertCommand.CommandText);

            _insertCommand.ExecuteNonQuery();
        }


        /// <summary>
        /// Updates a record in the database tables specified previously at <see cref="StartUpdating(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        /// <exception cref="Korzh.DbUtils.DbBridgeException">Seeding is not stared. Call StartUpdating() before.</exception>
        public void UpdateRecord(IDataRecord record)
        {
            if (CurrentTable is null || _updateCommand is null) {
                throw new DbBridgeException("Updating is not stared. Call StartUpdating() before.");
            }

            CheckConnection();

            FillParameters(_insertCommand, record);

            Logger?.LogDebug(_insertCommand.CommandText);

            _insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        /// <exception cref="Korzh.DbUtils.DbBridgeException">Seeding is not stared. Call StartSeeding() before.</exception>
        [Obsolete("Use InsertRecort() instead")]
        public void WriteRecord(IDataRecord record)
        {
            if (CurrentTable is null || _insertCommand is null) {
                throw new DbBridgeException("Seeding is not stared. Call StartSeeding() before.");
            }

            CheckConnection();

            FillParameters(_insertCommand, record);

            Logger?.LogDebug(_insertCommand.CommandText);

            _insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Generates the INSERT statement.
        /// </summary>
        /// <param name="table">The table we would like to insert a new record.</param>
        /// <param name="command">The command for which we generate our INSERT statement for</param>
        /// <returns>System.String.</returns>
        protected string GenerateInsertStatement(DatasetInfo table, IDbCommand command)
        {
            var sb = new StringBuilder(100);
            sb.AppendFormat("INSERT INTO {0} ( ", GetTableFullName(table));

            var columns = new List<ColumnInfo>();
            ExtractColumnList(table.Schema, table.Name, columns);

            for (var i = columns.Count - 1; i >= 0; i--) {
                //ignore not found columns and the columns which are auto-updated by DB (like Timestamps)
                if (!table.Columns.ContainsKey(columns[i].Name) || columns[i].IsTimestamp) {
                    columns.RemoveAt(i);
                }
            }

            foreach(var column in columns) {
                sb.AppendFormat("{0}{1}{2}, ", Quote1, column.Name, Quote2);
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(") VALUES ( ");

            foreach (var column in columns) {
                var paramName = ToParameterName(column.Name);
                _parameterColumnMap[paramName] = column.Name;
                sb.AppendFormat("{0}, ", paramName);
                var parameter = command.CreateParameter();
                parameter.DbType = column.DataType.ToDbType();
                //parameter.DbType = column.DbType; ????  maybe we need to save DbType on ExtractColumnList
                parameter.ParameterName = paramName;
                command.Parameters.Add(parameter);
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(");");

            return sb.ToString();
        }

        /// <summary>
        ///  Generates the UPDATE statement.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        protected string GenerateUpdateStatement(DatasetInfo table, IDbCommand command)
        {
            var sb = new StringBuilder(1024);
            sb.AppendFormat("UPDATE {0} SET ", GetTableFullName(table));

            var columns = new List<ColumnInfo>();
            ExtractColumnList(table.Schema, table.Name, columns);

            foreach (var column in columns.Where(c => !c.IsPrimaryKey)) {
                var paramName = ToParameterName(column.Name);
                _parameterColumnMap[paramName] = column.Name;
                var parameter = command.CreateParameter();
                parameter.DbType = column.DataType.ToDbType();
                //parameter.DbType = column.DbType; ????  maybe we need to save DbType on ExtractColumnList
                parameter.ParameterName = paramName;
                command.Parameters.Add(parameter);
                sb.AppendFormat("{0}{1}{2}={3}, ", Quote1, column.Name, Quote2.Length, paramName);
            }

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" WHERE ");
            foreach (var column in columns.Where(c => c.IsPrimaryKey)) {
                var paramName = ToParameterName(column.Name);
                _parameterColumnMap[paramName] = column.Name;
                var parameter = command.CreateParameter();
                parameter.DbType = column.DataType.ToDbType();
                //parameter.DbType = column.DbType; ????  maybe we need to save DbType on ExtractColumnList
                parameter.ParameterName = paramName;
                command.Parameters.Add(parameter);
                sb.AppendFormat("{0}{1}{2}={3}, ", Quote1, column.Name, Quote2.Length, paramName);
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(";");

            return sb.ToString();
        }

        /// <summary>
        /// Adds the parameters to the DB command object according to the current server type.
        /// </summary>
        /// <param name="command">The DB command.</param>
        /// <param name="record">The record. Each field in this record will be added a parameter.</param>
        /// <summary>
        /// Adds the parameters to the DB command object.
        /// </summary>
        protected void FillParameters(IDbCommand command, IDataRecord record)
        {
            foreach (var prmObj in command.Parameters) {
                var parameter = prmObj as IDataParameter;
                var ordinal = record.GetOrdinal(_parameterColumnMap[parameter.ParameterName]);
                parameter.Value = ordinal > -1 ? record.GetValue(ordinal) : DBNull.Value;
            }
        }


        /// <summary>
        /// Gets a correct name for the parameter.
        /// </summary>
        /// <param name="name">The base name for the parameter.</param>
        /// <returns>System.String.</returns>
        protected string ToParameterName(string name)
        {
            return "@" + name.ToLowerInvariant().Replace(' ', '_');
        }

        /// <summary>
        /// The current table we are going to seed with the data.
        /// </summary>
        protected DatasetInfo CurrentTable;

        /// <summary>
        /// Starts the seeding process for the specified table
        /// </summary>
        /// <param name="table">The table.</param>
        /// <exception cref="Korzh.DbUtils.DbBridgeException">Seeding is not finised. Call FinishSeeding() before start another one.</exception>
        public void StartSeeding(DatasetInfo table)
        {
            if (CurrentTable != null) {
                throw new DbBridgeException("Seeding is not finised. Call FinishSeeding() before start another one.");
            }

            CurrentTable = table;
            Logger?.LogDebug("Start seeding: " + GetTableFullName(CurrentTable));
            TurnOffConstraints();
            TurnOffAutoIncrement();

            _insertCommand = GetConnection().CreateCommand();
            _insertCommand.CommandText = GenerateInsertStatement(CurrentTable, _insertCommand);
            _insertCommand.CommandType = CommandType.Text;
        }

        /// <summary>
        /// Starts the updating process for the specified table
        /// </summary>
        /// <param name="table"></param>
        public void StartUpdating(DatasetInfo table) 
        {
            if (CurrentTable != null) {
                throw new DbBridgeException("Updating is not finised. Call FinishUpdating() before start another one.");
            }

            CurrentTable = table;
            Logger?.LogDebug("Start updating: " + GetTableFullName(CurrentTable));

            _updateCommand = GetConnection().CreateCommand();
            _updateCommand.CommandText = GenerateUpdateStatement(CurrentTable, _updateCommand);
            _updateCommand.CommandType = CommandType.Text;
        }

        private IDbCommand _insertCommand = null;
        private IDbCommand _updateCommand = null;

        private Dictionary<string, string> _parameterColumnMap = new Dictionary<string, string>();

        /// <summary>
        /// Sends an SQL command which turns off the constraints for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected abstract void TurnOffConstraints();

        /// <summary>
        /// Sends an SQL command which turns off the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected abstract void TurnOffAutoIncrement();

        /// <summary>
        /// Sends an SQL command which turns the constraints on for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected abstract void TurnOnConstraints();

        /// <summary>
        /// Sends an SQL command which turns on the possibility to set values for IDENTITY (auto-increment) columns for the current table.
        /// Must be implemented in derived classes.
        /// </summary>
        protected abstract void TurnOnAutoIncrement();

        /// <summary>
        /// Finilizes the seeding process.
        /// </summary>
        public void FinishSeeding()
        {
            TurnOnConstraints();
            TurnOnAutoIncrement();
            Logger?.LogDebug("Finish seeding: " + GetTableFullName(CurrentTable));
            CurrentTable = null;
            _insertCommand.Dispose();
            _insertCommand = null;
            _parameterColumnMap.Clear();
        }

        /// <summary>
        /// Finilizes the updating process.
        /// </summary>
        public void FinishUpdating()
        {
            Logger?.LogDebug("Finish updating: " + GetTableFullName(CurrentTable));
            CurrentTable = null;
            _updateCommand.Dispose();
            _updateCommand = null;
            _parameterColumnMap.Clear();
        }

        /// <summary>
        /// Gets the full name of the table (including schema and all necessary quotes).
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetTableFullName(DatasetInfo table)
        {
            var result = "";
            if (!string.IsNullOrEmpty(table.Schema)) {
                result += Quote1 + table.Schema + Quote2 + ".";
            }

            result += Quote1 + table.Name + Quote2;

            return result;
        }
    }

    /// <summary>
    /// Represents errors that occur during database operations.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DbBridgeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbBridgeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DbBridgeException(string message) : base(message) { }
    }
}
