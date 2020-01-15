using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Korzh.DbTool
{
    public static class ConnectionFactory
    {
        public static DbConnection Create(ConnectionInfo info)
        {
            switch (info.DbType)
            {
                case DbType.OldSqlServer:
                case DbType.SqlServer:
                    return new SqlConnection(info.ConnectionString);
                case DbType.MySql:
                    return new MySqlConnection(info.ConnectionString);
                case DbType.PostgreSql:
                    return new NpgsqlConnection(info.ConnectionString);
                default:
                    throw new Exception($"Unknown connection type: {info.DbType}. Evaluable types: {string.Join(", ", DbType.AllDbTypes)}");
            }

        }
    }
}
