using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Korzh.DbUtils.Tests
{
    public static class Common
    {

        public class TestItem
        {
            public string Name { get; set; }

            public int Price { get; set; }

            public string URL { get; set; }

            public byte[] Binary { get; set; }
        }

        private static List<TestItem> _testData = new List<TestItem>
        {
            new TestItem {
                Name = "Cyberpunk",
                Price = 2077,
                URL = "you\\are\\breathtaking",
                Binary = new byte[] { 0x20, 0x22 } 

            },
            new TestItem {
                Name = "Anno",
                Price = 2070,
                URL = "explore\\unknown",
                Binary = new byte[] { 0x18, 0x00 }

            },
            new TestItem {
                Name = "Dystopia",
                Price = 451,
                URL = "brave\\new\\word",
                Binary = new byte[] { 0x19, 0x84 }

            }
        };

        public static DataTable GenerateDataTableForTest()
        {
            var dt = new DataTable();

            dt.Columns.Add("Name", typeof(string)); ;
            dt.Columns.Add("Price", typeof(int));
            dt.Columns.Add("URL", typeof(string));
            dt.Columns.Add("Binary", typeof(byte[]));

            foreach (var item in _testData) {
                dt.Rows.Add(item.Name, item.Price, item.URL, item.Binary);
            }

            return dt;
        }

        public static List<TestItem> GetTestData()
        {
            return _testData;
        }
    }
}
