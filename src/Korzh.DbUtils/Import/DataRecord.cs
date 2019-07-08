using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Korzh.DbUtils
{

    public class DataRecord: IDataRecord
    {

        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        private readonly List<string> _keys = new List<string>();

        public int FieldCount => _properties.Count;

        public object this[string name] => _properties[name];

        public object this[int i] => _properties[_keys[i]];

        public void SetProperty(string name, string value)
        {
            _properties[name] = value;
            _keys.Add(name);
        }

        public void SetProperty(string name, object value)
        {
            _properties[name] = value.ToString();
            _keys.Add(name);
        }

        public bool GetBoolean(int i)
        {
            var value = _properties[_keys[i]];
            return bool.Parse(value);
        }

        public byte GetByte(int i)
        {
            var value = _properties[_keys[i]];
            return byte.Parse(value);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            var value = _properties[_keys[i]];
            var charArray = value.ToCharArray();

            if (charArray.LongLength > 0) {
                return charArray[0];
            }

            return '\0';
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var value = _properties[_keys[i]];
            var charArray = value.ToCharArray();

            long count = 0;
            for (var index = fieldoffset; index < charArray.LongLength; index++) {
                buffer[bufferoffset - 1 + count] = charArray[index];
            }

            return count;
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            string value = _properties[_keys[i]];
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(value, out result);
            return result;
        }

        public decimal GetDecimal(int i)
        {
            var value = _properties[_keys[i]];
            return decimal.Parse(value);
        }

        public double GetDouble(int i)
        {
            var value = _properties[_keys[i]];
            return double.Parse(value);
        }

        public Type GetFieldType(int i)
        {
            return typeof(string);
        }

        public float GetFloat(int i)
        {
            var value = _properties[_keys[i]];
            return float.Parse(value);
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            var value = _properties[_keys[i]];
            return short.Parse(value);
        }

        public int GetInt32(int i)
        {
            var value = _properties[_keys[i]];
            return int.Parse(value);
        }

        public long GetInt64(int i)
        {
            var value = _properties[_keys[i]];
            return long.Parse(value);
        }

        public string GetName(int i)
        {
            return _keys[i];
        }

        public int GetOrdinal(string name)
        {
            return _keys.IndexOf(name);
        }

        public string GetString(int i)
        {
            return _properties[_keys[i]];
        }

        public object GetValue(int i)
        {
            return _properties[_keys[i]];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            var value = _properties[_keys[i]];
            return value == null;
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
                return ValueAsShort(value);
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
