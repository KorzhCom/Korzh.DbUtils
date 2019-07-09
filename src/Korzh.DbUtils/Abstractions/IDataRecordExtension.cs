using System;
using System.Data;

namespace Korzh.DbUtils
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

                if (type.IsNullable()
                    && dataRecord.IsDBNull(i))
                {

                    value = null;
                    return true;
                }

                if (type == typeof(byte)
                  || type == typeof(byte?))
                {
                    value = dataRecord.GetByte(i);
                    return true;
                }

                if (type == typeof(byte[])) {

                    long size = dataRecord.GetBytes(i, 0, null, 0, 0); //get the length of data 
                    var result = new byte[size];
                    int bufferSize = 1024;
                    long bytesRead = 0;
                    int curPos = 0;
                    while (bytesRead < size)
                    {
                        bytesRead += dataRecord.GetBytes(i, curPos, result, curPos, bufferSize);
                        curPos += bufferSize;
                    }
                    value = result;
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
                    value = dataRecord.GetBoolean(i);
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

                if (type == typeof(Guid)
                    || type == typeof(Guid?)) {
                    value = dataRecord.GetGuid(i);
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
                value = type.GetDefaultValue();
                return false;
            }
        }

    }
}
