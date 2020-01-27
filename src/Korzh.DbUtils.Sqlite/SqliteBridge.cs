using System;
using System.Collections.Generic;
using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Korzh.DbUtils.Sqlite
{
    public class SqliteBridge : BaseDbBridge
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqliteBridge(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SqliteBridge(string connectionString, ILoggerFactory loggerFactory)
            : base(connectionString, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SqliteBridge(SqliteConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBridge"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SqliteBridge(SqliteConnection connection, ILoggerFactory loggerFactory)
            : base(connection, loggerFactory)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        protected override void ExtractColumnList(string tableSchema, string tableName, IList<ColumnInfo> columns)
        {
            throw new NotImplementedException();
        }

        protected override void ExtractDatasetList(IList<DatasetInfo> datasets)
        {
            throw new NotImplementedException();
        }

        protected override void TurnOffAutoIncrement()
        {
            throw new NotImplementedException();
        }

        protected override void TurnOffConstraints()
        {
            throw new NotImplementedException();
        }

        protected override void TurnOnAutoIncrement()
        {
            throw new NotImplementedException();
        }

        protected override void TurnOnConstraints()
        {
            throw new NotImplementedException();
        }
    }
}
