using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.EntityFrameworkCore.InMemory
{
    /// <summary>
    /// A default implementation of the main DB related intefaces for EFCore in memory provider.
    /// <see cref="Korzh.DbUtils.IDbReader" /> and <see cref="Korzh.DbUtils.IDbWriter" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDbReader" />
    /// <seealso cref="Korzh.DbUtils.IDbWriter" />
    public class DbContextBridge : IDbReader, IDbWriter
    {

        /// <summary>
        /// The DbContext.
        /// </summary>
        protected DbContext DbContext;

        /// <summary>
        /// Contains mapping between tables and entity types
        /// </summary>
        protected Dictionary<string, IEntityType> TableEntityTypes = new Dictionary<string, IEntityType>();

        /// <summary>
        /// The seeding or updating Table
        /// </summary>
        protected DatasetInfo CurrentTable;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextBridge"/> class.
        /// </summary>
        /// <param name="dbContext">The dbcontext.</param>
        /// <exception cref="DbContextBridgeException">This bridge supports only InMemory provider.</exception>
        public DbContextBridge(DbContext dbContext)
        {
            if (!dbContext.Database.IsInMemory()) {
                throw new DbContextBridgeException("This bridge supports only InMemory provider");
            }

            DbContext = dbContext;

            var entityTypes = DbContext.Model.GetEntityTypes();
            foreach (var entityType in entityTypes) {
                TableEntityTypes[entityType.GetTableName()] = entityType;
            }
         
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextBridge"/> class.
        /// </summary>
        /// <param name="dbContext">The dbcontext.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <exception cref="DbContextBridgeException">Throw if current provider is not im-memory.</exception>
        public DbContextBridge(DbContext dbContext, ILoggerFactory loggerFactory): this(dbContext)
        {
            Logger = loggerFactory?.CreateLogger("Korzh.DbUtils");
        }

        /// <summary>
        /// Gets the list of tables for the current dbcontext.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;DatasetInfo&gt;.</returns>
        public IReadOnlyCollection<DatasetInfo> GetDatasets()
        {
            var entityTypes = DbContext.Model.GetEntityTypes();
            var tables = new List<DatasetInfo>(entityTypes.Count());

            foreach (var entityType in entityTypes) {
                var tableName = entityType.GetTableName();
                var schemaName = entityType.GetSchema();

                var dataset = new DatasetInfo(tableName, schemaName);

                foreach (var property in entityType.GetProperties()) {
                    var name = property.GetColumnName();
                    var type = property.ClrType;
                    var isPK = property.IsPrimaryKey();
              
                    dataset.AddColumn(new ColumnInfo(name, type, isPK));
                }

                tables.Add(dataset);
            }

            return tables.AsReadOnly();
        }

        /// <summary>
        /// Inserts a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        /// <exception cref="DbContextBridgeException">Seeding is not stared. Call StartSeeding() before.</exception>
        [Obsolete("Use InsertRecort() instead")]
        public void WriteRecord(IDataRecord record)
        {
            InsertRecord(record);
        }

        /// <summary>
        /// Inserts a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        /// <exception cref="DbContextBridgeException">Seeding is not stared. Call StartSeeding() before.</exception>
        public void InsertRecord(IDataRecord record)
        {
            if (CurrentTable is null) {
                throw new DbBridgeException("Updating is not stared. Call StartUpdating() before.");
            }

            if (!TableEntityTypes.TryGetValue(CurrentTable.Name, out var entityType))
                throw new DbContextBridgeException("The table doesn't exist: " + CurrentTable.Name);


            var item = CreateEntityItem(entityType, record);

            DbContext.Add(item);
            DbContext.SaveChanges();

        }
        /// <summary>
        /// Updates a record in the database tables specified previously at <see cref="StartUpdating(DatasetInfo)" /> method call.
        /// </summary>
        /// <param name="record">The record to update in the database table.</param>
        /// <exception cref="DbContextBridgeException">Updating is not stared. Call StartUpdating() before.</exception>
        public void UpdateRecord(IDataRecord record)
        {
            if (CurrentTable is null) {
                throw new DbBridgeException("Updating is not stared. Call StartUpdating() before.");
            }

            if (!TableEntityTypes.TryGetValue(CurrentTable.Name, out var entityType))
                throw new DbContextBridgeException("The table doesn't exist: " + CurrentTable.Name);

            var item = CreateEntityItem(entityType, record);

            DbContext.Update(item);
            DbContext.SaveChanges();
        }

        private object CreateEntityItem(IEntityType entityType, IDataRecord record) 
        {

            var item = Activator.CreateInstance(entityType.ClrType);
            foreach (var property in entityType.GetProperties()) {

                if (property.IsConcurrencyToken)
                    continue;

                if (record.TryGetProperty(property.GetColumnName(), property.ClrType, out var propValue)) {
                    property.PropertyInfo.SetValue(item, propValue);
                }
            }

            return item;
        }

        /// <summary>
        /// Starts the seeding process for the specified table
        /// </summary>
        /// <param name="table">The table.</param>
        /// <exception cref="DbContextBridgeException">Seeding is not finised. Call FinishSeeding() before start another one.</exception>
        public void StartSeeding(DatasetInfo table)
        {
            if (!(CurrentTable is null)) {
                throw new DbContextBridgeException("Seeding is not finised. Call FinishSeeding() before start another one.");
            }

            CurrentTable = table;

            Logger?.LogDebug("Start seeding: " + CurrentTable.Name);
        }

        /// <summary>
        /// Finilizes the seeding process.
        /// </summary>
        public void FinishSeeding()
        {
            Logger?.LogDebug("Finish seeding: " + CurrentTable.Name);
            CurrentTable = null;
        }

        /// <summary>
        /// Starts the updating process for the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <exception cref="DbContextBridgeException">Updating is not finised. Call FinishUpdating() before start another one.</exception>
        public void StartUpdating(DatasetInfo table)
        {
            if (!(CurrentTable is null)) {
                throw new DbContextBridgeException("Updating is not finised. Call FinishUpdating() before start another one.");
            }

            CurrentTable = table;

            Logger?.LogDebug("Start updating: " + CurrentTable.Name);
        }

        /// <summary>
        /// Finilizes the updating process.
        /// </summary>
        public void FinishUpdating()
        {
            CurrentTable = null;

            Logger?.LogDebug("Finish updating: " + CurrentTable.Name);
        }
        /// <summary>
        /// Creates and returns a <see cref="IDataReader" /> object for some table.
        /// </summary>
        /// <param name="table">The table represented by <see cref="DatasetInfo" /> structure.</param>
        /// <returns>IDataReader.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public IDataReader GetDataReaderForTable(DatasetInfo table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executes the SQL statement passed in the parameter and returns a <see cref="IDataReader" /> object with the result of that execution.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <returns>IDataReader.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public IDataReader GetDataReaderForSql(string sql)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public IDbConnection GetConnection()
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Represents errors that occur during database operations.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DbContextBridgeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextBridgeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DbContextBridgeException(string message) : base(message) { }
    }

}
