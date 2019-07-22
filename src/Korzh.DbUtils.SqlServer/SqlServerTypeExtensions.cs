using System.Data;

namespace System
{
    /// <summary>
    /// Contains an extension method to get a SqlDbType from an instance of <see cref="System.Type"/> 
    /// </summary>
    public static class SqlServerTypeExtensions
    {
        /// <summary>
        /// Converts a <see cref="System.Type"/> to a <see cref="SqlDbType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>SqlDbType.</returns>
        public static SqlDbType ToSqlDbType(this Type type)
        {
            if (type.IsBool())
                return SqlDbType.Bit;

            if (type.IsByte())
                return SqlDbType.Char;

            if (type.IsInt16())
                return SqlDbType.SmallInt;

            if (type.IsInt32())
                return SqlDbType.Int;

            if (type.IsInt64())
                return SqlDbType.BigInt;

            if (type.IsFloat() || type.IsDouble())
                return SqlDbType.Float;

            if (type.IsDecimal())
                return SqlDbType.Decimal;

            if (type == typeof(string))
                return SqlDbType.Text;

            if (type.IsChar())
                return SqlDbType.Char;

            if (type == typeof(byte[]))
                return SqlDbType.VarBinary;

            if (type.IsDateTime())
                return SqlDbType.DateTime;

            if (type.IsDateTimeOffset())
                return SqlDbType.DateTime2;

            return SqlDbType.Text;
        }
    }
}
