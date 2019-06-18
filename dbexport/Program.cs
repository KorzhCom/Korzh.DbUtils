using System;

using dbexport.DbExporters;
using dbexport.Savers;

namespace dbexport
{
    class Program
    {
        static void Main(string[] args)
        {
            var msSqlServerCS = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EqDemoDb05;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var mySqlCS = "Server=demodb.korzh.com;Port=6603;Database=nwind;Uid=equser;Pwd=ILoveEasyQuery;SslMode=none";

            var dbExporter = new DbExporterBuilder()
                                 //.SetConnectionString(msSqlServerCS)
                                 .SetConnectionString(mySqlCS)
                                 //.UseDbExporter<MsSqlServerExporter>()
                                 .UseDbExporter<MySqlExporter>()
                                 .UseFileDbSaver("result-mysql.xml")
                                 .Build();

            dbExporter.Export();
        }

    }
}
