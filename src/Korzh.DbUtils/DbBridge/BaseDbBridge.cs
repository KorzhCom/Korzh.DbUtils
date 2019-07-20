using System;
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

            Logger?.LogInformation(command.CommandText);

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
            return GetDataReaderForSql("SELECT * FROM " + GetTableFullName(table));
        }

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
        /// Writes (adds) a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        /// <exception cref="Korzh.DbUtils.DbBridgeException">Seeding is not stared. Call StartSeeding() before.</exception>
        public void WriteRecord(IDataRecord record)
        {
            if (CurrentSeedingTable == null) {
                throw new DbBridgeException("Seeding is not stared. Call StartSeeding() before." );
            }

            var connection = GetConnection();

            var command = connection.CreateCommand();
            command.CommandText = GenerateInsertStatement(CurrentSeedingTable, record);
            command.CommandType = CommandType.Text;

            AddParameters(command, record);

            Logger?.LogInformation(command.CommandText);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Generates the INSERT statement.
        /// </summary>
        /// <param name="table">The table we would like to insert a new record.</param>
        /// <param name="record">The record to insert.</param>
        /// <returns>System.String.</returns>
        protected string GenerateInsertStatement(DatasetInfo table, IDataRecord record)
        {
            var sb = new StringBuilder(100);
            sb.AppendFormat("INSERT INTO {0} ( ", GetTableFullName(table));

            for (var i = 0; i < record.FieldCount; i++) {
                sb.AppendFormat("{0}{1}{2}, ", Quote1, record.GetName(i), Quote2);
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(") VALUES ( ");

            for (var i = 0; i < record.FieldCount; i++) {
                sb.AppendFormat("{0}, ", ToParameterName(record.GetName(i)));
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(");");

            return sb.ToString();
        }

        /// <summary>
        /// Adds the parameters to the DB command object according to the current server type.
        /// </summary>
        /// <param name="command">The DB command.</param>
        /// <param name="record">The record. Each field in this record will be added a parameter.</param>
        protected abstract void AddParameters(IDbCommand command, IDataRecord record);


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
        protected DatasetInfo CurrentSeedingTable;

        /// <summary>
        /// Starts the seeding process for the specified table
        /// </summary>
        /// <param name="table">The table.</param>
        /// <exception cref="Korzh.DbUtils.DbBridgeException">Seeding is not finised. Call FinishSeeding() before start another one.</exception>
        public void StartSeeding(DatasetInfo table)
        {
            if (CurrentSeedingTable != null) {
                throw new DbBridgeException("Seeding is not finised. Call FinishSeeding() before start another one.");
            }

            CurrentSeedingTable = table;
            Logger?.LogInformation("Start seeding: " + GetTableFullName(CurrentSeedingTable));
            TurnOffConstraints();
            TurnOffAutoIncrement();
        }

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
            Logger?.LogInformation("Finish seeding: " + GetTableFullName(CurrentSeedingTable));
            CurrentSeedingTable = null;
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
        public DbBridgeException(string message) : base(message) { }
    }
}
