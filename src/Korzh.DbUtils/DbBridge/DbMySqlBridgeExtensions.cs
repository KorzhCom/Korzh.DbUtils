namespace Korzh.DbUtils
{
    public static class DbMySqlBridgeExtensions
    {
        public static void UseMySQL(this DbInitializerOptions options, string connectionString)
        {
            options.DbWriter = new Korzh.DbUtils.MySql.MySqlBride(connectionString);
        }
    }
}
