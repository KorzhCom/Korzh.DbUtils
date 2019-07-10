
using MySql.Data.MySqlClient;

namespace System
{
    public static class TypeMySqlExtensions
    {
        public static MySqlDbType ToMySqlDbType(this Type type)
        {
            if (type.IsBool())
                return MySqlDbType.Bit;

            if (type.IsByte())
                return MySqlDbType.Byte;

            if (type.IsInt16())
                return MySqlDbType.Int16;

            if (type.IsInt32())
                return MySqlDbType.Int32;

            if (type.IsInt64())
                return MySqlDbType.Int64;

            if (type.IsFloat())
                return MySqlDbType.Float;

            if (type.IsDouble())
                return MySqlDbType.Double;

            if (type.IsDecimal())
                return MySqlDbType.Decimal;

            if (type == typeof(string))
                return MySqlDbType.Text;

            if (type.IsChar())
                return MySqlDbType.VarChar;

            if (type == typeof(byte[]))
                return MySqlDbType.VarBinary;

            if (type.IsDateTime() || type.IsDateTimeOffset())
                return MySqlDbType.DateTime;

            return MySqlDbType.Text;
        }
    }
}
