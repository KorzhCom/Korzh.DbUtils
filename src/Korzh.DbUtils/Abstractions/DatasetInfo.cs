using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.DbUtils
{
    public class DatasetInfo
    {
        public string Name { get; private set; }

        public DatasetInfo(string name)
        {
            Name = name;
        }
    }
}
