using System;
using Microsoft.EntityFrameworkCore;

using Korzh.DbUtils;
using Korzh.DbUtils.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextBridgeExtensions
    {
        public static void UseDbContext(this DbInitializerOptions options, DbContext dbContext, bool useMigrations) 
        {

            options.DbWriter = new DbContextBridge(dbContext);

            if (useMigrations) {
                options.NeedDataSeeding = dbContext.Database.GetPendingMigrations() == dbContext.Database.GetMigrations();
                dbContext.Database.Migrate();
            }
            else {
                options.NeedDataSeeding = dbContext.Database.EnsureCreated();
            }
        }
    }
}
