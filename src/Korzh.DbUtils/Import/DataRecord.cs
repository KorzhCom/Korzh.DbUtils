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
            _properties[name] = value?.ToString();
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
    }
}
