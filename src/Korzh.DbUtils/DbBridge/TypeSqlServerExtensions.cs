using System.Data;

namespace System
{
    public static class TypeSqlServerExtensions
    {
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
