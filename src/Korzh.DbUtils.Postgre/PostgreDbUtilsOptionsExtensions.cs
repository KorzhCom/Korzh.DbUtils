namespace Korzh.DbUtils.Postgre
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
        public static void UsePostgre(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new PostgreBridge(connectionString, options.LoggerFactory);
        }
    }
}
