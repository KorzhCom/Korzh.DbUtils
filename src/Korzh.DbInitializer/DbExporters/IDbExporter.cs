using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Korzh.DbInitializer.DbExporters
{
    public interface IDbExporter
    {
        void Export();
    }
}
