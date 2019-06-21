using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Korzh.DbInitializer
{

    public interface IDataItem
    {
        IReadOnlyDictionary<string, string> Properties { get; }

        bool TryGetProperty<T>(string name, out T value);

        bool TryGetProperty(string name, Type type, out object value)
    }

    public class DataItem: IDataItem
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> Properties => _properties;

        public void SetProperty(string name, string value)
        {
            _properties[name] = value;
        }

        public void SetProperty(string name, object value)
        {
            _properties[name] = value.ToString();
        }

        public bool TryGetProperty<T>(string name, out T value)
        {
            var result = TryGetProperty(name, typeof(T), out var val);
            if (result) {
                value = (T)val;
            }
            else {
                value = default(T);
            }

            return result;
        }

        public bool TryGetProperty(string name, Type type, out object value)
        {
            if (_properties.TryGetValue(name, out var propVal))
            {

                value = ToObject(propVal, type);

                return true;
            }

            value = GetDefaultValue(type);

            return false;
        }

        private static object ToObject(string value, Type type)
        {
            if (type == typeof(string))
            {
                return value;
            }

            if (string.IsNullOrEmpty(value))
            {
                return GetDefaultValue(type);
            }

            if (type == typeof(int) ||
               type == typeof(int?))
            {
                return ValueAsInt(value);
            }

            if (type == typeof(short) ||
            type == typeof(short?))
            {
                return ValueAsInt(value);
            }

            if (type == typeof(bool) ||
            type == typeof(bool?))
            {
                return ValueAsBool(value);
            }

            if (type == typeof(float) ||
            type == typeof(float?))
            {
                return ValueAsFloat(value);
            }

            if (type == typeof(double) ||
            type == typeof(double?))
            {
                return ValueAsDouble(value);
            }

            if (type == typeof(decimal) ||
            type == typeof(decimal?))
            {
                return ValueAsDecimal(value);
            }

            if (type == typeof(DateTime) ||
            type == typeof(DateTime?))
            {
                return ValueAsDateTime(value);
            }

            if (type == typeof(DateTimeOffset) ||
            type == typeof(DateTimeOffset?))
            {
                return ValueAsDateTimeOffset(value);
            }

            if (type == typeof(TimeSpan) ||
            type == typeof(TimeSpan?))
            {
                return ValueAsTimeSpan(value);
            }

            return null;
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

        private static int ValueAsInt(string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }

        private static bool ValueAsBool(string value)
        {
            bool result = false;
            bool.TryParse(value, out result);
            return result;
        }

        private static decimal ValueAsDecimal(string value)
        {
            return decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static short ValueAsShort(string value)
        {
            return short.Parse(value);
        }

        private static float ValueAsFloat(string value)
        {
            return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static double ValueAsDouble(string value)
        {
            return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static DateTime ValueAsDateTime(string value)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(value, out result);
            return result;
        }

        private static TimeSpan ValueAsTimeSpan(string value)
        {
            TimeSpan result = TimeSpan.MinValue;
            TimeSpan.TryParse(value, out result);
            return result;
        }

        private static DateTimeOffset ValueAsDateTimeOffset(string value)
        {
            DateTimeOffset result = DateTimeOffset.MinValue;
            DateTimeOffset.TryParse(value, out result);
            return result;
        }
    }
}
