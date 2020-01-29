using Microsoft.Data.Sqlite;

namespace System
{
    /// <summary>
    /// Contains an extension method to get a <see cref="SqliteType"/> from an instance of <see cref="System.Type"/> 
    /// </summary>
    public static class SqliteTypeExtensions
    {
        /// <summary>
        /// Converts a <see cref="System.Type"/> to a <see cref="SqliteType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>NpgsqlDbType.</returns>
        public static SqliteType ToSqliteDbType(this Type type)
        {
            if (type.IsBool())
                return SqliteType.Integer;

            if (type.IsByte())
                return SqliteType.Integer;

            if (type.IsInt16())
                return SqliteType.Integer;

            if (type.IsInt32())
                return SqliteType.Integer;

            if (type.IsInt64())
                return SqliteType.Integer;

            if (type.IsFloat())
                return SqliteType.Real;

            if (type.IsDouble())
                return SqliteType.Real;

            if (type.IsDecimal())
                return SqliteType.Real;

            if (type == typeof(string))
                return SqliteType.Text;

            if (type.IsChar())
                return SqliteType.Text;

            if (type == typeof(byte[]))
                return SqliteType.Blob;

            if (type.IsDateTime())
                return SqliteType.Integer;

            if (type.IsDateTimeOffset())
                return SqliteType.Integer;

            return SqliteType.Text;
        }
    }
}
