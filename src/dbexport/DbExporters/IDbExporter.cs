using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace dbexport.DbExporters
{
    internal interface IDbExporter
    {
        void Export();
    }
}
