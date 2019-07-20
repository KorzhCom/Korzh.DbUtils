namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering MySqlBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class DbMySqlBridgeExtensions
    {
        /// <summary>
        /// Registers MySqlBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void UseMySQL(this IDbUtilsOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.MySql.MySqlBridge(connectionString, options.LoggerFactory);
        }
    }
}
