using Microsoft.Data.Sqlite;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering SqliteBridge as a DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class SqliteDbUtilsOptionsExtensions
    {
        /// <summary>
        /// Registers SqliteBridge as a DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void UseSqlite(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.Sqlite.SqliteBridge(connectionString, options.LoggerFactory);
        }

        /// <summary>
        /// Registers SqliteBridge as a DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connection">The connection.</param>
        public static void UseSqlite(this IDbUtilsOptions options, SqliteConnection connection)
        {
            options.DbWriter = new Korzh.DbUtils.Sqlite.SqliteBridge(connection, options.LoggerFactory);
        }
    }
}
