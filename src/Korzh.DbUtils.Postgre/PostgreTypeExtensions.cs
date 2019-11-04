using NpgsqlTypes;

namespace System
{
    /// <summary>
    /// Contains an extension method to get a <see cref="NpgsqlDbType"/> from an instance of <see cref="System.Type"/> 
    /// </summary>
    public static class PostgreTypeExtensions
    {
        /// <summary>
        /// Converts a <see cref="System.Type"/> to a <see cref="NpgsqlDbType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>NpgsqlDbType.</returns>
        public static NpgsqlDbType ToPostgreDbType(this Type type)
        {
            if (type.IsBool())
                return NpgsqlDbType.Bit;

            if (type.IsByte())
                return NpgsqlDbType.Char;

            if (type.IsInt16())
                return NpgsqlDbType.Smallint;

            if (type.IsInt32())
                return NpgsqlDbType.Integer;

            if (type.IsInt64())
                return NpgsqlDbType.Bigint;

            if (type.IsFloat())
                return NpgsqlDbType.Real; 

            if (type.IsDouble())
                return NpgsqlDbType.Double;

            if (type.IsDecimal())
                return NpgsqlDbType.Numeric;

            if (type == typeof(string))
                return NpgsqlDbType.Text;

            if (type.IsChar())
                return NpgsqlDbType.Char;

            if (type == typeof(byte[]))
                return NpgsqlDbType.Bytea;

            if (type.IsDateTime())
                return NpgsqlDbType.Date;

            if (type.IsDateTimeOffset())
                return NpgsqlDbType.TimeTz;

            return NpgsqlDbType.Text;
        }
    }
}
