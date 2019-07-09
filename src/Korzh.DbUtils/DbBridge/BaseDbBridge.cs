using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Korzh.DbUtils
{
    public abstract class BaseDbBridge : IDbReader, IDbWriter
    {

        protected DbConnection Connection = null;
        private readonly string _connectionString;

        public BaseDbBridge(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected BaseDbBridge(DbConnection connection)
        {
            Connection = connection;
            _connectionString = connection.ConnectionString;
        }

        protected void CheckConnection()
        {
            if (Connection == null) {
                Connection = CreateConnection(_connectionString);
            }

            if (Connection.State != ConnectionState.Open) {
                Connection.Open();
            }
        }

        protected abstract DbConnection CreateConnection(string connectionString);

        public IDbConnection GetConnection()
        {
            CheckConnection();
            return Connection;
        }


        public IDataReader GetDataReaderForSql(string sql)
        {
            var connection = GetConnection();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }

        protected virtual string Quote1 => "[";

        protected virtual string Quote2 => "]";

        public IDataReader GetDataReaderForTable(string tableName)
        {
            return GetDataReaderForSql("SELECT * FROM " + Quote1 + tableName + Quote2);
        }

        public IReadOnlyCollection<DatasetInfo> GetDatasets()
        {
            CheckConnection();
            var tables = new List<DatasetInfo>();

            ExtractDatasetList(tables);

            return tables.AsReadOnly();
        }


        protected abstract void ExtractDatasetList(IList<DatasetInfo> datasets);

        public void WriteRecord(DatasetInfo table, IDataRecord record)
        {
            var connection = GetConnection();

            var command = connection.CreateCommand();
            command.CommandText = GenerateInsertStatement(table, record);
            command.CommandType = CommandType.Text;

            AddParameters(command, record);

            command.ExecuteNonQuery();
        }

        protected string GenerateInsertStatement(DatasetInfo table, IDataRecord record)
        {
            var sb = new StringBuilder(100);
            sb.AppendFormat("INSERT INTO {0} ( ", GetTableFullName(table));

            for (var i = 0; i < record.FieldCount; i++) {
                sb.AppendFormat("{0}{1}{2}, ", Quote1, record.GetName(i), Quote2);
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(") VALUES ( ");

            for (var i = 0; i < record.FieldCount; i++) {
                sb.AppendFormat("{0}, ", ToParameterName(record.GetName(i)));
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(");");

            return sb.ToString();

        }

        protected abstract void AddParameters(IDbCommand command, IDataRecord record);
      

        protected string ToParameterName(string name)
        {
            return "@" + name.ToLowerInvariant().Replace(' ', '_');
        }

        public void StartSeeding(DatasetInfo table)
        {
            TurnOffContraints(table);
            TurnOffAutoIncrement(table);
        }

        protected abstract void TurnOffContraints(DatasetInfo table);

        protected abstract void TurnOffAutoIncrement(DatasetInfo table);

        protected abstract void TurnOnContraints(DatasetInfo table);

        protected abstract void TurnOnAutoIncrement(DatasetInfo table);

        public void FinishSeeding(DatasetInfo table)
        {
            TurnOnContraints(table);
            TurnOnAutoIncrement(table);
        }

        protected virtual string GetTableFullName(DatasetInfo table)
        {
            var result = "";
            if (!string.IsNullOrEmpty(table.Schema)) {
                result += Quote1 + table.Schema + Quote2 + ".";
            }

            result += Quote1 + table.Name + Quote2;

            return result;
        }
    }
}
