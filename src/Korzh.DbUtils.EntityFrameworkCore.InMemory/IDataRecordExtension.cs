using System;
using System.Data;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Contains several useful extension methods of IDataRecord interface.
    /// </summary>
    public static class IDataRecordExtension
    {
        /// <summary>
        /// Reads property from data record if it exists.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="i">The property index.</param>
        /// <param name="value">The property value.</param>
        /// <returns></returns>
        public static bool TryGetProperty<T>(this IDataRecord dataRecord, int i, out T value)
        {
            var result = TryGetProperty(dataRecord, i, typeof(T), out var valueObj);
            value = (T)valueObj;
            return result;
        }

        /// <summary>
        /// Reads property from data record if it exists.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns></returns>
        public static bool TryGetProperty<T>(this IDataRecord dataRecord, string name, out T value)
        {
            return TryGetProperty(dataRecord, dataRecord.GetOrdinal(name), out value);
        }

        /// <summary>
        /// Reads property from data record if it exists.
        /// </summary>
        /// <param name="dataRecord">The data rectord.</param>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="value">The property value.</param>
        /// <returns></returns>
        public static bool TryGetProperty(this IDataRecord dataRecord, string name, Type type, out object value)
        {
            return TryGetProperty(dataRecord, dataRecord.GetOrdinal(name), type, out value);
        }

        /// <summary>
        /// Reads property from data record if it exists.
        /// </summary>
        /// <param name="dataRecord">The data rectord.</param>
        /// <param name="i">The property index.</param>
        /// <param name="type">The property type.</param>
        /// <param name="value">The property value.</param>
        /// <returns></returns>
        public static bool TryGetProperty(this IDataRecord dataRecord, int i, Type type, out object value)
        {
            try {

                if (type == typeof(string)) {
                    value = dataRecord.GetString(i);
                    return true;
                }

                if (type.IsNullable()
                    && dataRecord.IsDBNull(i)) {

                    value = null;
                    return true;
                }

                if (type.IsByte()) {
                    value = dataRecord.GetByte(i);
                    return true;
                }

                if (type == typeof(byte[])) {

                    long size = dataRecord.GetBytes(i, 0, null, 0, 0); //get the length of data 
                    var result = new byte[size];
                    int bufferSize = 1024;
                    long bytesRead = 0;
                    int curPos = 0;
                    while (bytesRead < size) {
                        bytesRead += dataRecord.GetBytes(i, curPos, result, curPos, bufferSize);
                        curPos += bufferSize;
                    }
                    value = result;
                    return true;
                }

                if (type.IsInt32()) {
                    value = dataRecord.GetInt32(i);
                    return true;
                }


                if (type.IsBool()) {
                    value = dataRecord.GetBoolean(i);
                    return true;
                }

                if (type.IsInt16()) {
                    value = dataRecord.GetInt16(i);
                    return true;
                }

                if (type.IsFloat()) {
                    value = dataRecord.GetFloat(i);
                    return true;
                }

                if (type.IsDouble()) {
                    value = dataRecord.GetDouble(i);
                    return true;
                }

                if (type.IsDecimal()) {

                    value = dataRecord.GetDecimal(i);
                    return true;
                }

                if (type.IsGuid()) {
                    value = dataRecord.GetGuid(i);
                    return true;
                }

                if (type.IsDateTime()) {
                    value = dataRecord.GetDateTime(i);
                    return true;
                }

                if (type.IsDateTimeOffset()) {
                    value = (DateTimeOffset)dataRecord.GetDateTime(i);
                    return true;
                }

                if (type.IsTimeSpan()) {
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
