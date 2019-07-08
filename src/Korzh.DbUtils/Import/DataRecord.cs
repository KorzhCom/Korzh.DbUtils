using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Korzh.DbUtils
{

    public class DataRecord: IDataRecord
    {

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        private readonly List<string> _keys = new List<string>();

        public int FieldCount => _properties.Count;

        public object this[string name] => _properties[name];

        public object this[int i] => _properties[_keys[i]];

        public void SetProperty(string name, object value)
        {
            _properties[name] = value;
            _keys.Add(name);
        }

        public void SetProperty(string name, Type valueType, string value)
        {
            SetProperty(name, value.CastToType(valueType));
        }

        public void SetProperty(string name, string valueType, string value)
        {
            SetProperty(name, Type.GetType(valueType), value);
        }

        public bool GetBoolean(int i)
        {
            return (bool)GetValue(i);
        }

        public byte GetByte(int i)
        {
            return (byte)GetValue(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)GetValue(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var value = GetString(i);
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
            return GetValue(i).GetType().ToString();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)GetValue(i);
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)GetValue(i);
        }

        public double GetDouble(int i)
        {
            return (double)GetValue(i);
        }

        public Type GetFieldType(int i)
        {
            return typeof(string);
        }

        public float GetFloat(int i)
        {
            return (float)GetValue(i);
        }

        public Guid GetGuid(int i)
        {
            return (Guid)GetValue(i);
        }

        public short GetInt16(int i)
        {
            return (short)GetValue(i);
        }

        public int GetInt32(int i)
        {
            return (int)GetValue(i);
        }

        public long GetInt64(int i)
        {
            return (long)GetValue(i);
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
            return (string)GetValue(i);
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
            var value = GetValue(i);
            return value == null;
        }

    }

    internal static class StringExtentions
    {

        public static object CastToType(this string value, Type type)
        {
            if (type == typeof(string))
                return value;

            //Possibly it would be better to rewrite
            //without this extensions
            if (value == null)
                return type.GetDefaultValue();

            if (type == typeof(int)
                || type == typeof(int?))
                return int.Parse(value);

            if (type == typeof(short)
               || type == typeof(short?))
                return short.Parse(value);

            if (type == typeof(byte)
               || type == typeof(byte?))
                return byte.Parse(value);

            if (type == typeof(long)
               || type == typeof(long?))
                return long.Parse(value);

            if (type == typeof(float)
              || type == typeof(float?))
                return float.Parse(value);

            if (type == typeof(double)
              || type == typeof(double?))
                return double.Parse(value);

            if (type == typeof(char)
              || type == typeof(char?))
                return double.Parse(value);

            //if (type == )

            if (type == typeof(DateTime)
               || type == typeof(DateTime?))
                return DateTime.Parse(value);

            if (type == typeof(DateTimeOffset)
               || type == typeof(DateTimeOffset?))
                return DateTimeOffset.Parse(value);

            if (type == typeof(Guid)
                || type == typeof(Guid))
                return Guid.Parse(value);

            if (type == typeof(TimeSpan)
                || type == typeof(TimeSpan))
                return TimeSpan.Parse(value);

            throw new InvalidCastException("Unknown type to case: " + type.ToString());

        }
    }
}
