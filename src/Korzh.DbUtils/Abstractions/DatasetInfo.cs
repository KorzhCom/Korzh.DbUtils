using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.DbUtils
{

    public class ColumnInfo
    {
        public string Name { get; private set; }

        public Type DataType {get; private set;}

        public string DataTypeName => DataType.ToString();

        public ColumnInfo(string name, string type)
        {
            Name = name;
            DataType = Type.GetType(type);
        }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            DataType = type;
        }

    }


    public class DatasetInfo
    {
        public string Name { get; private set; }

        public string Schema { get; private set; }

        public IReadOnlyDictionary<string, ColumnInfo> Columns => _columns;

        private Dictionary<string, ColumnInfo> _columns = new Dictionary<string, ColumnInfo>();

        public DatasetInfo(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        public void AddColumn(ColumnInfo column)
        {
            _columns[column.Name] = column;
        }
    }
}
