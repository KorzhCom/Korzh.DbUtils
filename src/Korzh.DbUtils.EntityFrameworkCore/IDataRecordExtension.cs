using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Korzh.DbUtils.EntityFrameworkCore
{
    public static class IDataRecordExtension
    {

        public static bool TryGetProperty<T>(this IDataRecord dataRecord, int i, out T value)
        {
            var result = TryGetProperty(dataRecord, i, typeof(T), out var valueObj);
            value = (T)valueObj;
            return result;
        }

        public static bool TryGetProperty<T>(this IDataRecord dataRecord, string name, out T value)
        {
            return TryGetProperty(dataRecord, dataRecord.GetOrdinal(name), out value);
        }

        public static bool TryGetProperty(this IDataRecord dataRecord, string name, Type type, out object value)
        {
            return TryGetProperty(dataRecord, dataRecord.GetOrdinal(name), type, out value);
        }


        public static bool TryGetProperty(this IDataRecord dataRecord, int i, Type type, out object value)
        {

            try {

                if (type == typeof(string)) {
                    value = dataRecord.GetString(i);
                    return true;
                }

                if (IsNullable(type)
                    && dataRecord.IsDBNull(i))
                {

                    value = null;
                    return true;
                }


                if (type == typeof(int)
                    || type == typeof(int?))
                {
                    value = dataRecord.GetInt32(i);
                    return true;
                }


                if (type == typeof(bool)
                    || type == typeof(bool?))
                {
                    value = dataRecord.GetInt16(i);
                    return true;
                }

                if (type == typeof(short)
                    || type == typeof(short?))
                {
                    value = dataRecord.GetInt16(i);
                    return true;
                }

                if (type == typeof(float)
                    || type == typeof(float?))
                {
                    value = dataRecord.GetFloat(i);
                    return true;
                }

                if (type == typeof(double)
                    || type == typeof(double?))
                {
                    value = dataRecord.GetDouble(i);
                    return true;
                }

                if (type == typeof(decimal)
                    || type == typeof(decimal?))
                {

                    value = dataRecord.GetDecimal(i);
                    return true;
                }

                if (type == typeof(DateTime)
                    || type == typeof(DateTime?)) {
                    value = dataRecord.GetDateTime(i);
                    return true;
                }

                if (type == typeof(DateTimeOffset)
                    || type == typeof(DateTimeOffset?)) {
                    value = (DateTimeOffset)dataRecord.GetDateTime(i);
                    return true;
                }


                if (type == typeof(TimeSpan)) {
                    value = TimeSpan.FromTicks(dataRecord.GetDateTime(i).Ticks);
                    return true;
                }

                if (type == typeof(TimeSpan) 
                    || type == typeof(TimeSpan?))
                {
                    value = TimeSpan.FromTicks(dataRecord.GetDateTime(i).Ticks);
                }

                value = dataRecord.GetValue(i);
                return true;
            }
            catch {
                value = GetDefaultValue(type);
                return false;
            }
        }
        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static object GetDefaultValue(Type type)
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

    }
}
