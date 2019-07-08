using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.DbUtils
{

    public class ColumnInfo
    {
        public string Name { get; private set; }

        public Type Type {get; private set;}

        public string TypeName => Type.ToString();

        public ColumnInfo(string name, string type)
        {
            Name = name;
            Type = Type.GetType(type);
        }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

    }


    public class DatasetInfo
    {
        public string Name { get; private set; }

        public IReadOnlyDictionary<string, ColumnInfo> Columns => _columns;

        private Dictionary<string, ColumnInfo> _columns = new Dictionary<string, ColumnInfo>();

        public DatasetInfo(string name)
        {
            Name = name;
        }

        public void AddColumn(ColumnInfo column)
        {
            _columns[column.Name] = column;
        }
    }
}
