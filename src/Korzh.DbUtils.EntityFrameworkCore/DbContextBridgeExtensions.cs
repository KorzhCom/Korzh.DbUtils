using System;
using Microsoft.EntityFrameworkCore;

using Korzh.DbUtils;
using Korzh.DbUtils.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextBridgeExtensions
    {
        public static void UseDbContext(this IDbUtilsOptions options, DbContext dbContext, bool useMigrations) 
        {

            options.DbWriter = new DbContextBridge(dbContext);

            if (useMigrations) {
                dbContext.Database.Migrate();
            }
          
        }
    }
}
