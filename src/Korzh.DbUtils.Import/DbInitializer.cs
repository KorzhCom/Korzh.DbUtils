using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

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


        public static DbInitializer Create(Action<DbInitializerOptions> initAction, ILoggerFactory loggerFactory = null)
        {
            var options = new DbInitializerOptions(loggerFactory);

            initAction?.Invoke(options);

            if (options.DatasetImporter == null) {
                options.DatasetImporter = new JsonDatasetImporter(options.LoggerFactory);
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

}
