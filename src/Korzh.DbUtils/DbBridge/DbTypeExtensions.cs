using System.Data;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// Contains an extension method to get a DbType from an instance of <see cref="System.Type"/> 
    /// </summary>
    public static class SqlServerTypeExtensions
    {
        /// <summary>
        /// Converts a <see cref="System.Type"/> to a <see cref="DbType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>SqlDbType.</returns>
        public static DbType ToDbType(this Type type)
        {
            if (type.IsBool())
                return DbType.Boolean;

            if (type.IsByte())
                return DbType.Byte;

            if (type.IsInt16())
                return DbType.Int16;

            if (type.IsInt32())
                return DbType.Int32;

            if (type.IsInt64())
                return DbType.Int64;

            if (type.IsFloat() )
                return DbType.Single;

            if (type.IsDouble())
                return DbType.Double;
            
            if (type.IsDecimal())
                return DbType.Decimal;

            if (type == typeof(string))
                return DbType.String;

            if (type.IsChar())
                return DbType.StringFixedLength;

            if (type == typeof(byte[]))
                return DbType.Binary;

            if (type.IsDateTime())
                return DbType.DateTime;

            if (type.IsDateTimeOffset())
                return DbType.DateTimeOffset;

            return DbType.String;
        }
    }
}