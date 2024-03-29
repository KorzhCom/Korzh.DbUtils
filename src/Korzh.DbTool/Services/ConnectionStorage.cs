﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;
using Korzh.DbUtils;

namespace Korzh.DbTool
{

    public class ConnectionInfo
    {
        public string Id { get; set; }

        public string DbType { get; set; }

        public string ConnectionString { get; set; }

        public string IncludeTables { get; set; }

        public string ExcludeTables { get; set; }

        public ConnectionInfo() { }

        public ConnectionInfo(string id, string dbType, string connectionString)
        {
            Id = id;
            DbType = dbType;
            ConnectionString = connectionString;
        }

        public Func<DatasetInfo, bool> GetDatasetFilter()
        {
            var includeTables = !string.IsNullOrEmpty(IncludeTables)
                    ? IncludeTables.Split(',').ToList()
                    : null;

            var excludeTables = !string.IsNullOrEmpty(ExcludeTables)
                            ? ExcludeTables.Split(',').ToList()
                            : new List<string>();

            Func<DatasetInfo, bool> filter = (dataSet) => {
                return (includeTables == null || includeTables.Contains(dataSet.Name))
                        && !excludeTables.Contains(dataSet.Name);
            };

            return filter;
        }
    }

    public class ConnectionListItem
    {
        public string ConnectionId { get; set; }

        public ConnectionInfo Info { get; set; }

        public ConnectionListItem(string connectionId, ConnectionInfo info)
        {
            ConnectionId = connectionId;
            Info = info;
        }
    }

    public class ConnectionStorage
    {

        private readonly string _configFile;

        private readonly Dictionary<string, ConnectionInfo> _connections;
             
        public ConnectionStorage(string configFile)
        {
            _configFile = configFile;


            if (File.Exists(_configFile)) {
                var config = JObject.Parse(File.ReadAllText(_configFile));
                _connections = config["connections"].ToObject<Dictionary<string, ConnectionInfo>>();
            }
            else {
                _connections = new Dictionary<string, ConnectionInfo>();
            }
               
        }

        public ConnectionInfo Get(string id)
        {
            if (_connections.TryGetValue(id, out var info)) {
                return info;
            }

            return null;
        }

        public void AddUpdate(string id, ConnectionInfo info)
        {
            _connections[id] = info;
        }

        public void Remove(string id)
        {
            _connections.Remove(id);
        }

        public List<ConnectionListItem> List()
        {
            return _connections.Select(c => new ConnectionListItem(c.Key, c.Value)).ToList();
        }

        public void SaveChanges()
        {
            JObject config;
            if (File.Exists(_configFile)) {
                config = JObject.Parse(File.ReadAllText(_configFile));
            }
            else {
                config = new JObject();
                Directory.CreateDirectory(Path.GetDirectoryName(_configFile));
            }

            config["connections"] = JObject.FromObject(_connections);
            File.WriteAllText(_configFile, config.ToString());
        }
    } 
}
