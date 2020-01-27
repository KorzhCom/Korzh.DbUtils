using Npgsql;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering SqlServerBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class PostgreDbUtilsOptionsExtensions
    {
        /// <summary>
        /// Registers PostgreBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void UsePostgreSql(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.PostgreSql.PostgreBridge(connectionString, options.LoggerFactory);
        }

        /// <summary>
        /// Registers PostgreBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connection">The connection.</param>
        public static void UsePostgreSql(this IDbUtilsOptions options, NpgsqlConnection connection)
        {
            options.DbWriter = new Korzh.DbUtils.PostgreSql.PostgreBridge(connection, options.LoggerFactory);
        }
    }
}
