namespace Korzh.DbUtils.SqlServer
{
    public static class DbMsSqlBridgeExtensions
    {
        public static void UseSqlServer(this DbInitializerOptions options, string connectionString)
        {
            options.DbWriter = new MsSqlBridge(connectionString);
        }
    }
}
