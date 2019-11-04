namespace Korzh.DbTool
{
    public static class DbType
    {
        public const string SqlServer = "mssql";

        public const string MySql = "mysql";

        public const string PostgreSql = "postgresql";

        public static readonly string[] AllDbTypes = new[] { DbType.SqlServer, DbType.MySql, DbType.PostgreSql };

    }
}
