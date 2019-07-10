namespace Korzh.DbUtils
{
    public static class DbSqlServerBridgeExtensions
    {
        public static void UseSqlServer(this DbInitializerOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.SqlServer.SqlServerBridge(connectionString, options.LoggerFactory);
        }
    }
}
