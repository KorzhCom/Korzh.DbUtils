using System;
using Microsoft.EntityFrameworkCore;

using Korzh.DbUtils;
using Korzh.DbUtils.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextBridgeExtensions
    {
        public static void UseDbContext<DbContextType>(this DbInitializerOptions options, IServiceProvider serviceProvider) 
            where DbContextType : DbContext
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()) { 
                var context = scope.ServiceProvider.GetService<DbContextType>();

                options.DbWriter = new DbContextBridge(context);
            }
        }
    }
}
