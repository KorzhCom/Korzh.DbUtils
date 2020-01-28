using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Korzh.DbUtils.EntityFrameworkCore.InMemory
{
    /// <summary>
    /// Defines some extensions which unify meta-data functions used in EF Core 2 with EF Core 3 API.
    /// </summary>
    public static class EntityFrameworkCore2Extensions
    {
        /// <summary>
        /// Gets the table name for the specified IEntityType.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>System.String.</returns>
        public static string GetTableName(this IEntityType entityType)
        {
            return entityType.Relational().TableName;
        }

        /// <summary>
        /// Gets the schema name for the specified IEntityType
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>System.String.</returns>
        public static string GetSchema(this IEntityType entityType)
        {
            return entityType.Relational().Schema;
        }

        /// <summary>
        /// Gets the column name for the specified IProperty.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public static string GetColumnName(this IProperty property)
        {
            return property.Relational().ColumnName;
        }
    }
}
