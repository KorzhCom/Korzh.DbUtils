
# DbTool utility and Korzh.DbUtils packages

This repository contains the sources for:

* Korzh.DbUtils library - a set of classes and packages for different manipulations with database data (export, import and data seeding)

* `dbtool` utility - a .NET Core global  tools which provides DB exporting/importing functions with a command-line interface.

This set of tools can help you to export the content of your database to some format by your choise (XML or JSON currently) and then use that exported data to seed your database on another machine in a simple and convenient way.

## DbTool utility

`dbtool` is .NET Core global tool, so installation is as simple as for any other global tool (provided that you already have [.NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet-core) or higher installed on your computer):

```bash
dotnet tool install -g Korzh.DbTool
```

### Adding a connection

__dbtool__ stores the information about DB connections and some other settings in a global configuration file ({USERDIR}/.korzh/dbtool.config), so register your connection in that list you need to call `connections add` command:

```bash
dbtool connections add {Connection ID} {DB Type (mssql|mysql)} {Connection string}
```

For example:

```bash
dbtool connections add demo1 mssql "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EqDemoDb07;Integrated Security=True;"
```

### Exporting DB data

Now, when the connection is defined you can export your DB data to some common format (JSON or XML for now):

```bash
dbtool export {Connection ID} [--format=xml|json] [--output=path] | [--zip=filename]
```

All options (`--format`, `--output` and `--zip`) can be omitted.
In this case the tool will use JSON format and will store all exported data in a `{Connection ID}-YYYYMMDD` folder (without ZIP packing).

For example:

```bash
dbtool export demo1 --format=xml --output=MyDbData --zip=MyDb.zip
```

## Korzh.DbUtils library

The libary contsists for several packages which implements some basic database operations:

* Korzh.DbUtils package
  Defines basic abstractions and interfaces like `IDatasetExporter`, `IDatasetImporter`, `IDataPacker`, `IDbBridge`

* Korzh.DbUtils.Import package
  Contains implementations or `IDatasetImporter` interface for XML and JSON formats. Additionally it contains DbInitializer class which can be used for data seeding in your projects.

* Korzh.DbUtils.Export package
  Contains implementations of `IDatasetExporter` for XML and JSON.

* Korzh.DbUtils.SqlServer
  Implements DB manipulation interfaces (`IDbBridge`, `IDbReader`, `IDbSeeder`) for MS SQL Server connections.

* Korzh.DbUtils.MySQL
  Implements DB manipulation interfaces for MySQL connections.


## Basic scenario: Data seeding in your app

Let's suppose you have a "master copy" of some DB which you need to duplicate on user's machine on the first start of your application. It's quite usual situation when you need to distribute some demonstration project for your class library (like we do a lot of time during our work on [EasyQuery library](https://korzh.com/easyquery)) or it's local database for you desktop application which must be seeded with some default data.

Here is how you solve this task with our DbUtils tool set.

### Step 1: Export you "master" DB

Just install `dbtool` as it's described above, add a connection to you DB and then export all data from it to some folder:

```bash
dotnet tool install -g Korzh.DbTool

dbtool connections add MyMasterDb mssql "{the connection string to your DB}"

dbtool export MyMasterDb
```

### Step 2: Add data files to your project

After the previous step you will have a new folder like `MyMasterDb-20190720` with a bunch of JSON files in it (one for each table). Copy all these files to your project's folder to `App_Data\DbSeed` sub-folder.
Please note, that you will also need to add those files to your project manually for .NET Framework projects.

### Step 3: DB initialization code

Let's suppose we have a ASP.NET Core project and we need to seed our DB with the data on the first start. The database itself is created automatically with Entity Framework Core migrations. To seed it with the data we just need:

#### 1. Install `Korzh.DbUtils.Import` NuGet package

#### 2. Add the initialization code

Here is an example of the such code we need to add at the end of `Startup.Configure` method:

```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    .     .     .     .

    app.UseMvc();

    using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    using (var context = scope.ServiceProvider.GetService<AppDbContext>()) {
        if (context.Database.EnsureCreated()) { //run only if database was not created previously
            Korzh.DbUtils.DbInitializer.Create(options => {
                options.UseSqlServer(Configuration.GetConnectionString("MyDemoDb")); //set the connection string for our database
                options.UseFileFolderPacker(System.IO.Path.Combine(env.ContentRootPath, "App_Data", "SeedData")); //set the folder where to get the seeding data
            })
            .Seed();
        }
    }
}
```

That's it. With the above 3 simple steps your database will be created and seeded automatically on the first app start.
