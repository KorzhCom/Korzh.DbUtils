using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.DbInitializer
{

    public interface IDataItem
    {
        IReadOnlyDictionary<string, string> Properties { get; }
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
    }
}
