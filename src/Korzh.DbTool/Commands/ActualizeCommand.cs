using System;
using System.Collections.Generic;
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
            command.Description = "This command can help you to actualize date/time values in some DB. " + 
                    "It's very usuful when you need to update some testing/demo database and make all dates in it actual (meainig: within this year or close)";
            command.HelpOption("-?|-h|--help");

            // add local config option
            command.AddOption(options.LocalConfigFilePathOption);

            var connectionIdArg= command.Argument("<connection ID>", "The ID of some previously registered connection")
                                        .IsRequired();

            var columnDateArg = command.Argument("<column>", "The full name of some date/time column to get the DELTA for all other date/time values in this DB. " 
                                                + "The DELTA is calculated as CurrentYear - MAX(<column>).Year. For example: Order.OrderDate")
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
                Console.WriteLine("Connection not found: " + connectionId);
                return -1;
            }

            InitConnection(info);

            var filter = info.GetDatasetFilter();

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
                if (filter(dataSet)) {
                    continue;
                }

                var count = 0;
                try {
                    Console.WriteLine($"Updating: {dataSet.Name}...");
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
