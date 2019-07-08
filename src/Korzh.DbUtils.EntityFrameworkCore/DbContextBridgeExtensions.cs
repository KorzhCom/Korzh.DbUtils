using System;
using Microsoft.EntityFrameworkCore;

using Korzh.DbUtils;
using Korzh.DbUtils.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextBridgeExtensions
    {
        public static void UseDbContext<DbContextType>(this DbInitializerOptions options, IServiceProvider serviceProvider, bool useMigrations) 
            where DbContextType : DbContext
        {

            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            options.DisposableObjects.Add(scope);

            var context = scope.ServiceProvider.GetService<DbContextType>();
            options.DisposableObjects.Add(context);

            options.DbWriter = new DbContextBridge(context);

            if (useMigrations) {
                options.NeedDataSeeding = context.Database.GetPendingMigrations() == context.Database.GetMigrations();
                context.Database.Migrate();
            }
            else {
                options.NeedDataSeeding = context.Database.EnsureCreated();
            }
        }
    }
}
