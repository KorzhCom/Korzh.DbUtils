using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Contains several useful extension methods for operations with <see cref="Type"/> objects.
    /// Most of the functions are self-descriptive.
    /// </summary>
    public static class TypeExtensions
    {
        public  static object GetDefaultValue(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Default(type), typeof(object)
                )
            );

            return e.Compile()();
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsBool(this Type type)
        {
            return type == typeof(bool) || type == typeof(bool);
        }

        public static bool IsByte(this Type type)
        {
            return type == typeof(byte) || type == typeof(byte);
        }

        public static bool IsChar(this Type type)
        {
            return type == typeof(char) || type == typeof(char);
        }

        public static bool IsInt16(this Type type)
        {
            return type == typeof(short) || type == typeof(short?);
        }

        public static bool IsInt32(this Type type)
        {
            return type == typeof(int) || type == typeof(int?);
        }

        public static bool IsInt64(this Type type)
        {
            return type == typeof(long) || type == typeof(long?);
        }

        public static bool IsDecimal(this Type type)
        {
            return type == typeof(decimal) || type == typeof(decimal?);
        }

        public static bool IsFloat(this Type type)
        {
            return type == typeof(float) || type == typeof(float?);
        }

        public static bool IsDouble(this Type type)
        {
            return type == typeof(double) || type == typeof(double?);
        }

        public static bool IsGuid(this Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        public static bool IsDateTime(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public static bool IsDateTimeOffset(this Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        public static bool IsTimeSpan(this Type type)
        {
            return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
        }
    }
}
