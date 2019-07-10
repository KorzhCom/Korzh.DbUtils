using System;
using System.Collections.Generic;
using Korzh.DbUtils.Import;
using Korzh.DbUtils.Packing;

namespace Korzh.DbUtils
{
    public enum DbBackupFormat {
        JSON,

        XML
    }


    public class DbInitializer : IDisposable
    {
        private readonly DbImporter _dbImporter;

        private DbInitializerOptions _options;

        public DbInitializer(DbInitializerOptions options)
        {
            _dbImporter = new DbImporter(options.DbWriter, options.DatasetImporter, options.Unpacker);
            _options = options;
        }

        public void Run()
        {
            if (_options.NeedDataSeeding) {
                _dbImporter.Import();
            }
        }


        public static DbInitializer Create(Action<DbInitializerOptions> initAction)
        {
            var options = new DbInitializerOptions();

            initAction?.Invoke(options);

            if (options.DatasetImporter == null) {
                options.DatasetImporter = new JsonDatasetImporter();
            }

            if (options.Unpacker == null) {
                options.Unpacker = new FileFolderPacker(options.InitialDataFolder);
            }

            return new DbInitializer(options);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public class DbInitializerOptions
    {
        public string InitialDataFolder { get; set; }

        public IDbWriter DbWriter { get; set; }

        public IDatasetImporter DatasetImporter { get; set; }

        public IDataUnpacker Unpacker { get; set; }

        public bool NeedDataSeeding { get; set; } = false;

        public DbInitializerOptions() {
            InitialDataFolder = System.IO.Path.Combine("App_Data", "InitialData");
        }
    }
}
