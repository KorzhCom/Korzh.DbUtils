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

        public void WriteRecord(string tableName, IDataRecord record)
        {
            var connection = GetConnection();

            var command = connection.CreateCommand();
            command.CommandText = GenerateInsertStatement(tableName, record);
            command.CommandType = CommandType.Text;

            AddParameters(command, record);

            command.ExecuteNonQuery();
        }

        protected string GenerateInsertStatement(string tableName, IDataRecord record)
        {
            var sb = new StringBuilder(100);
            sb.AppendFormat("INSERT INTO {0} ( ", Quote1 + tableName + Quote2);

            for (var i = 0; i < record.FieldCount; i++) {
                sb.AppendFormat("{0}, ", record.GetName(i));
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

        public void StartSeeding()
        {
            // Get all constraints and save them
            // Turn all constraints off
            TurnOffContraints();
            TurnOffAutoIncrement();
        }

        protected abstract void TurnOffContraints();

        protected abstract void TurnOffAutoIncrement();

        protected abstract void TurnOnContraints();

        protected abstract void TurnOnAutoIncrement();

        public void FinishSeeding()
        {
            TurnOnContraints();
            TurnOnAutoIncrement();
            //Turn all saved constraints on
            //Clear the list of constraints
        }
    }
}
