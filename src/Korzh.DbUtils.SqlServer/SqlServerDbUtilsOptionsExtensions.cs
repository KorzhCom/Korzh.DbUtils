using System.Data.SqlClient;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering SqlServerBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class SqlServerDbUtilsOptionsExtensions
    {
        /// <summary>
        /// Registers SqlServerBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">Different options. An object that implements <see cref="IDbUtilsOptions"/> interface.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void UseSqlServer(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.SqlServer.SqlServerBridge(connectionString, options.LoggerFactory);
        }

        /// <summary>
        /// Registers SqlServerBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connection">The connection.</param>
        public static void UseSqlServer(this IDbUtilsOptions options, SqlConnection connection)
        {
            options.DbWriter = new Korzh.DbUtils.SqlServer.SqlServerBridge(connection, options.LoggerFactory);
        }
    }
}
