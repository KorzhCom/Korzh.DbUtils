using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Korzh.DbUtils.EntityFrameworkCore
{
    public static class IDataRecordExtension
    {

        public static bool TryGetProperty<T>(this IDataRecord dataRecord, string name, out T value)
        {
            var result = TryGetProperty(dataRecord, name, typeof(T),out var valueObj);
            value = (T)valueObj;
            return result;
        }
             
        public static bool TryGetProperty(this IDataRecord dataRecord, string name, Type type, out object value)
        {
            // To Do move logic from DataRecord here
            value = null;
            return false;
        }


    }
}
