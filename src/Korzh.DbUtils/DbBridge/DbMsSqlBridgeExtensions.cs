using System;

using Korzh.DbUtils;
using Korzh.DbUtils.SqlServer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbMsSqlBridgeExtensions
    {
        public static void UseSqlServer(this DbInitializerOptions options, string connectionString)
        {
            options.DbWriter = new MsSqlBridge(connectionString);
        }
    }
}
