using Microsoft.Data.Sqlite;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering SqliteBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class SqliteDbUtilsOptionsExtensions
    {
        /// Registers MySqlBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void UseSqlServer(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.Sqlite.SqliteBridge(connectionString, options.LoggerFactory);
        }

        /// Registers MySqlBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connection">The connection.</param>
        public static void UseSqlServer(this IDbUtilsOptions options, SqliteConnection connection)
        {
            options.DbWriter = new Korzh.DbUtils.Sqlite.SqliteBridge(connection, options.LoggerFactory);
        }
    }
}
