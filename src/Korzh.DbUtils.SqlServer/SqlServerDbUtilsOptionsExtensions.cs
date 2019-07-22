namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering SqlServerBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class SqlServerDbUtilsOptionsExtensions
    {
        /// <summary>
        /// Registers MySqlBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void UseSqlServer(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.SqlServer.SqlServerBridge(connectionString, options.LoggerFactory);
        }
    }
}
