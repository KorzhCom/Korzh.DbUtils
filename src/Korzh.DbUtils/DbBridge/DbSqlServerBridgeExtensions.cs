using Korzh.DbUtils.SqlServer;

namespace Korzh.DbUtils
{
    public static class DbSqlServerBridgeExtensions
    {
        public static void UseSqlServer(this DbInitializerOptions options, string connectionString)
        {
            options.DbWriter = new SqlServerBridge(connectionString, options.LoggerFactory);
        }
    }
}
