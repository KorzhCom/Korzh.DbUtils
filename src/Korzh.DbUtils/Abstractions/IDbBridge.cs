using System;
using System.Collections.Generic;
using System.Data;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Defines the basic database operations
    /// </summary>
    public interface IDbBridge
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Gets the list of tables (or other kind of dataset) for the current connection.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;DatasetInfo&gt;.</returns>
        IReadOnlyCollection<DatasetInfo> GetDatasets();
    }

    /// <summary>
    /// Defines reading database operations 
    /// Implements the <see cref="Korzh.DbUtils.IDbBridge" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDbBridge" />
    public interface IDbReader : IDbBridge
    {
        /// <summary>
        /// Creates and returns a <see cref="IDataReader"/> object for some table.
        /// This method usually just constructs a correct SQL statement to get the content of some table (like 'SELECT * FROM TableName)
        /// and then calls <see cref="GetDataReaderForSql(string)"/> function.
        /// </summary>
        /// <param name="table">The table represented by <see cref="DatasetInfo"/> structure.</param>
        /// <returns>IDataReader.</returns>
        IDataReader GetDataReaderForTable(DatasetInfo table);

        /// <summary>
        /// Executes the SQL statement passed in the parameter and returns a <see cref="IDataReader"/> object with the result of that execution.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <returns>IDataReader.</returns>
        IDataReader GetDataReaderForSql(string sql);
    }

    /// <summary>
    /// Defines the operations for seeding data to some database table
    /// Implements the <see cref="Korzh.DbUtils.IDbBridge" />
    /// </summary>
    /// <seealso cref="Korzh.DbUtils.IDbBridge" />
    public interface IDbWriter : IDbBridge
    {

        /// <summary>
        /// Writes (adds) a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)"/> method call. 
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        [Obsolete("Use InsertRecord() instead.")]
        void WriteRecord(IDataRecord record);

        /// <summary>
        /// Inserts a record to the database tables specified previously at <see cref="StartSeeding(DatasetInfo)"/> method call. 
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        void InsertRecord(IDataRecord record);

        /// <summary>
        /// Updates a record in the database tables specified previously at <see cref="StartUpdating(DatasetInfo)"/> method call. 
        /// </summary>
        /// <param name="record">The record to save to the database table.</param>
        void UpdateRecord(IDataRecord record);

        /// <summary>
        /// Starts the seeding process for the specified table
        /// </summary>
        /// <param name="table">The table.</param>
        void StartSeeding(DatasetInfo table);

        /// <summary>
        /// Starts the updating process for the specified table
        /// </summary>
        /// <param name="table">The table.</param>
        void StartUpdating(DatasetInfo table);

        /// <summary>
        /// Finilizes the seeding process.
        /// </summary>
        void FinishSeeding();

        /// <summary>
        /// Finilizes the updating process.
        /// </summary>
        void FinishUpdating();
    }
}
