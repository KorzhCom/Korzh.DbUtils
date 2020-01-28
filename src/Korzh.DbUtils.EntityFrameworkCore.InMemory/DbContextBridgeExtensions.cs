using Microsoft.EntityFrameworkCore;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Static class with extensions for registering DbContextBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>
    /// </summary>
    public static class DbContextBridgeExtensions
    {

        /// <summary>
        /// Registers DbContextBridge as DB reader and DB writer in <see cref="IDbUtilsOptions"/>.
        /// SUPPORTS ONLY IN-MEMORY PROVIDER.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dbContext"></param>
        /// <exception cref="Korzh.DbUtils.EntityFrameworkCore.InMemory.DbContextBridgeException">This bridge supports only InMemory provider</exception>
        public static void UseInMemoryDatabase(this IDbUtilsOptions options, DbContext dbContext) 
        {
            options.DbWriter = new Korzh.DbUtils.EntityFrameworkCore.InMemory.DbContextBridge(dbContext, options.LoggerFactory);
        }
    }
}
