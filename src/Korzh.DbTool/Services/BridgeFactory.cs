using System.Data.Common;
using System.Data.SqlClient;

using MySql.Data.MySqlClient;
using Npgsql;

using Korzh.DbUtils;
using Korzh.DbUtils.MySql;
using Korzh.DbUtils.PostgreSql;
using Korzh.DbUtils.SqlServer;

namespace Korzh.DbTool
{
    public static class DbBridgeFactory
    {
        public static BaseDbBridge Create(DbConnection connection)
        {
            if (connection is SqlConnection) {
                return new SqlServerBridge(connection as SqlConnection, Program.LoggerFactory);
            }
            else if (connection is MySqlConnection) {
                return new MySqlBridge(connection as MySqlConnection, Program.LoggerFactory);
            }
            else if (connection is NpgsqlConnection) {
                return new PostgreBridge(connection as NpgsqlConnection, Program.LoggerFactory);
            }

            return null;
        }
    }
}
