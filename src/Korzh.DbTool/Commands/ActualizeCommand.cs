using System;
using System.Data;
using System.Data.Common;
using System.Linq;

using McMaster.Extensions.CommandLineUtils;

using Korzh.DbUtils;

namespace Korzh.DbTool
{
    class ActualizeCommand : ICommand
    {

        public static void Configure(CommandLineApplication command, GlobalOptions options)
        {
            command.Description = "Our INTERNAL command to actualize test (demo) data. Current update dates value updating using calculated delta.";
            command.HelpOption("-?|-h|--help");

            // add local config option
            command.Options.Add(options.LocalConfigFilePathOption);

            var connectionIdArg= command.Argument("<connection ID>", "The ID of some previously registered connection")
                                        .IsRequired();

            var columnDateArg = command.Argument("<column date>", "The column date to calculate delta as CurrentYear - MAX(<column date>).Year. For example Order.OrderDate")
                                       .IsRequired();

            Func<int> runCommandFunc = new ActualizeCommand(connectionIdArg, columnDateArg, options).Run;
            command.OnExecute(runCommandFunc);
        }

        private readonly GlobalOptions _options;
        private readonly CommandArgument _connectionIdArg;
        private readonly CommandArgument _columnDateArg;

        public ActualizeCommand(CommandArgument connectionIdArg, CommandArgument columnDateArg, GlobalOptions options)
        {
            _connectionIdArg = connectionIdArg;
            _columnDateArg = columnDateArg;
            _options = options;
        }

        private DbConnection _connection;

        private void InitConnection(ConnectionInfo info)
        {
            _connection = ConnectionFactory.Create(info);

            if (_connection.State != ConnectionState.Open) {
                Console.WriteLine($"Openning {info.Id} connection...");
                _connection.Open();
            }
        }

        public int Run()
        {

            var connectionId = _connectionIdArg.Value;
            var storage = new ConnectionStorage(_options.ConfigFilePath);
            var info = storage.Get(connectionId);
            if (info == null) {
                Console.WriteLine("Connection with current ID is not found: " + connectionId);
                return -1;
            }

            InitConnection(info);

            Func<DatasetInfo, bool> filter = null;
            if (!string.IsNullOrEmpty(info.Tables)) {
                var tables = info.Tables.Split(',').ToList();
                filter = (dataSet) =>
                {
                    return tables.Contains(dataSet.Name);
                };
            }

            var bridgeSelect = DbBridgeFactory.Create(_connection);
            var bridgeUpdate = DbBridgeFactory.Create(ConnectionFactory.Create(info));

            var tableNameDate = _columnDateArg.Value.Substring(0, _columnDateArg.Value.IndexOf('.'));
            var columnNameDate = _columnDateArg.Value.Substring(_columnDateArg.Value.IndexOf('.') + 1);

            var dataSets = bridgeSelect.GetDatasets();
            var mainDataSet = dataSets.First(ds => ds.Name.Equals(tableNameDate, StringComparison.InvariantCultureIgnoreCase));

            var reader = bridgeSelect.GetDataReaderForSql($"SELECT MAX({bridgeSelect.GetFormattedColumnName(columnNameDate)}) FROM {bridgeSelect.GetFormattedTableName(mainDataSet)}");
            reader.Read();
            var maxDate = reader.GetDateTime(0);
            reader.Close();

            var deltaYear = DateTime.Now.Year - maxDate.Year;
            Console.WriteLine("Delta year: " + deltaYear);

            if (deltaYear < 0)
                return 0;

            foreach (var dataSet in dataSets) {
                if (!(filter is null || filter(dataSet))) {
                    continue;
                }

                var count = 0;
                try {
                    Console.WriteLine("Updating: " + dataSet.Name);
                    bridgeUpdate.StartUpdating(dataSet);
                    using (reader = bridgeSelect.GetDataReaderForTable(dataSet)) {
                        while (reader.Read()) {
                            var record = new DataRecord();
                            FillDataRecord(record, reader, deltaYear);
                            bridgeUpdate.UpdateRecord(record);
                            count++;
                        }
                    }
                }
                finally {
                    Console.WriteLine("Updated: " + count);
                    bridgeUpdate.FinishUpdating();
                }
            }

            return 0;
        }

        private void FillDataRecord(DataRecord record, IDataReader reader, int deltaYear)
        {
            for (var i = 0; i < reader.FieldCount; i++) {
                var columnName = reader.GetName(i);
                var value = reader.GetValue(i);
                var type = reader.GetFieldType(i);

                if (!(value is null) && value != DBNull.Value) {
                    if (type.IsDateTime()) {
                        value = ((DateTime)value).AddYears(deltaYear);
                    }
                    else if (type.IsDateTimeOffset()) {
                        value = ((DateTimeOffset)value).AddYears(deltaYear);
                    }
                }

                record[columnName] = value;          
            }
           
        }
    }
}
