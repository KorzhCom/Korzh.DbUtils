using System.Data.Common;
using System.Data.SqlClient;

using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;

using Korzh.DbUtils;
using Korzh.DbUtils.MySql;
using Korzh.DbUtils.PostgreSql;
using Korzh.DbUtils.SqlServer;
using Korzh.DbUtils.Sqlite;

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
            else if (connection is SqliteConnection) {
                return new SqliteBridge(connection as SqliteConnection, Program.LoggerFactory);
            }

            return null;
        }
    }
}
