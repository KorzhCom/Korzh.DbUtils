namespace Korzh.DbTool
{
    public static class DbType
    {
        public const string SqlServer = "sqlserver";

        public const string OldSqlServer = "mssql"; //back capability

        public const string MySql = "mysql";

        public const string PostgreSql = "postgre";

        public const string Sqlite = "sqlite";

        public static readonly string[] AllDbTypes = new[] { DbType.SqlServer, DbType.MySql, DbType.PostgreSql, DbType.Sqlite };

    }
}
