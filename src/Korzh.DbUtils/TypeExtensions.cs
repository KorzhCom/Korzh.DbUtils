using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Contains several useful extension methods for operations with <see cref="Type"/> objects.
    /// Most of the functions are self-descriptive.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentNullException">type</exception>
        public static object GetDefaultValue(this Type type)
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

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is nullable; otherwise, <c>false</c>.</returns>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether the specified type is a boolean.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a boolean; otherwise, <c>false</c>.</returns>
        public static bool IsBool(this Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        /// <summary>
        /// Determines whether the specified type is a byte type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a byte type; otherwise, <c>false</c>.</returns>
        public static bool IsByte(this Type type)
        {
            return type == typeof(byte) || type == typeof(byte?);
        }

        /// <summary>
        /// Determines whether the specified type is a character type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a character type; otherwise, <c>false</c>.</returns>
        public static bool IsChar(this Type type)
        {
            return type == typeof(char) || type == typeof(char?);
        }

        /// <summary>
        /// Determines whether the specified type is a short integer type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a short int type; otherwise, <c>false</c>.</returns>
        public static bool IsInt16(this Type type)
        {
            return type == typeof(short) || type == typeof(short?)
                    || type == typeof(UInt16) || type == typeof(UInt16?);
        }

        /// <summary>
        /// Determines whether the specified type is an integer type ((either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is an int type; otherwise, <c>false</c>.</returns>
        public static bool IsInt32(this Type type)
        {
            return type == typeof(int) || type == typeof(int?)
                    || type == typeof(UInt32) || type == typeof(UInt32?);
        }

        /// <summary>
        /// Determines whether the specified type is a big integer type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a short int type; otherwise, <c>false</c>.</returns>
        public static bool IsInt64(this Type type)
        {
            return type == typeof(long) || type == typeof(long?) 
                    || type == typeof(UInt64) || type == typeof(UInt64?);
        }

        /// <summary>
        /// Determines whether the specified type is a decimal (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is decimal; otherwise, <c>false</c>.</returns>
        public static bool IsDecimal(this Type type)
        {
            return type == typeof(decimal) || type == typeof(decimal?);
        }

        /// <summary>
        /// Determines whether the specified type is a float (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is float; otherwise, <c>false</c>.</returns>
        public static bool IsFloat(this Type type)
        {
            return type == typeof(float) || type == typeof(float?);
        }

        /// <summary>
        /// Determines whether the specified type is a double type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is double; otherwise, <c>false</c>.</returns>
        public static bool IsDouble(this Type type)
        {
            return type == typeof(double) || type == typeof(double?);
        }

        /// <summary>
        /// Determines whether the specified type is a GUID type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a GUID type; otherwise, <c>false</c>.</returns>
        public static bool IsGuid(this Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        /// <summary>
        /// Determines whether the specified type is a DateTime type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a DateTime type; otherwise, <c>false</c>.</returns>
        public static bool IsDateTime(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        /// <summary>
        /// Determines whether the specified type is a DateTimeOffset type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a DateTimeOffset type; otherwise, <c>false</c>.</returns>
        public static bool IsDateTimeOffset(this Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        /// <summary>
        /// Determines whether the specified type is a TimeSpan type (either nullable or non-nullable).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a TimeSpan type; otherwise, <c>false</c>.</returns>
        public static bool IsTimeSpan(this Type type)
        {
            return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
        }
    }
}
