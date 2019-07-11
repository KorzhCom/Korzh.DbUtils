using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
#pragma warning disable EF1000
        public static int SaveChangesWithIdentity(this DbContext dbContext, params IEntityType[] entityTypes)
        {
            dbContext.Database.OpenConnection();
            var baseSql = "SET IDENTITY_INSERT ";

            var tableNames = entityTypes.Select(et => {
                var etr = et.Relational();
                var tableSchema = etr.Schema;
                return string.IsNullOrEmpty(tableSchema)
                    ? etr.TableName
                    : tableSchema + "." + etr.TableName + "";
            });

            foreach (var tableName in tableNames) {
                dbContext.Database.ExecuteSqlCommand(baseSql + tableName + " ON");
            }
            var result = dbContext.SaveChanges();

            foreach (var tableName in tableNames) {
                dbContext.Database.ExecuteSqlCommand(baseSql + tableName + " OFF");
            }
            return result;
        }
#pragma warning restore EF1000
    }
}
