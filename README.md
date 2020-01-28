
# DbTool utility and Korzh.DbUtils packages

| Build status | Nuget| Inspired by|
|---|---|---|
|   [![Build status](https://korzhdev.visualstudio.com/EasyQuery/_apis/build/status/Kedonec/prod-DbUtils)](https://korzhdev.visualstudio.com/EasyQuery/_build/latest?definitionId=47) | [![NuGet](https://img.shields.io/nuget/v/Korzh.DbTool.svg)](https://www.nuget.org/packages/Korzh.DbTool) |[![EasyQuery](https://i.ibb.co/xzsf24t/Easy-Query.png)](http://korzh.com/easyquery)|

## About

This repository contains the sources for:

* Korzh.DbUtils library - a set of classes and packages for different manipulations with database data (export, import, data seeding).

* `dbtool` utility - a .NET Core global tool which provides DB exporting/importing functions via a command-line interface.

This set of tools can help you with exporting your database content to some format (XML or JSON currently) and then use that exported data to seed your database on another machine in a simple and convenient way.

## DbTool utility

`dbtool` is .NET Core global tool, so installation is as simple as for any other global tool (provided that you already have [.NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet-core) or higher installed on your computer):

```cmd
dotnet tool install -g Korzh.DbTool
```

### Adding a connection

__dbtool__ stores the information about DB connections and some other settings in a global configuration file ({USERDIR}/.korzh/dbtool.config), so register your connection in that list you need to call `connections add` command:

```cmd
dbtool connections add {Connection ID} {DB Type (sqlserver|mysql|postgre|sqlite)} {Connection string}
```

For example:

```cmd
dbtool connections add demo1 sqlserver "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EqDemoDb07;Integrated Security=True;"
```

### Exporting DB data

Now, when the connection is defined you can export your DB data to some common format (JSON or XML for now):

```cmd
dbtool export {Connection ID} [--format=xml|json] [--output=path] [--zip=filename]
```

All options (`--format`, `--output` and `--zip`) can be omitted.
In this case the tool will use JSON format and will store all exported data in a `{Connection ID}-YYYYMMDD` folder (without ZIP packing).

For example the following command:

```cmd
dbtool export demo1 --format=xml --output=MyDbData
```

will export your DB to a bunch of XML files and then put those files into MyDbData folder.

### Importing data to DB

You can import the data created on the previous step back to your DB. Or to any other DB with the same structure.

> NB: DbTool does not create tables during the importing operation. So you database must exist already and has the same (or at least a similar) structure as the original one.

Here is how your `import` command should look like:

```cmd
dbtool import {Connection ID} [--input=path] [--format=xml|json]
```

`--input` option tells utility to search for the data by the specified path. If that path is a folder - then it will look for .xml or .json files in that folder. If it's a ZIP file - then it will unpack that archive first and take necessary data files from there.

`--format` can be omitted since DbTool can recognize the format by files' extensions.

Example:

```cmd
dbtool import demo1 --input=MyDbData.zip
```

## Korzh.DbUtils library

The library includes several packages which implement some basic database operations:

* `Korzh.DbUtils`

  Defines basic abstractions and interfaces like `IDatasetExporter`, `IDatasetImporter`, `IDataPacker`, `IDbBridge`

* `Korzh.DbUtils.Import`

  Contains implementations or `IDatasetImporter` interface for XML and JSON formats. Additionally, it contains DbInitializer class which can be used for data seeding in your projects.

* `Korzh.DbUtils.Export`

  Contains implementations of `IDatasetExporter` for XML and JSON.

* `Korzh.DbUtils.SqlServer`

  Implements DB manipulation interfaces (`IDbBridge`, `IDbReader`, `IDbSeeder`) for MS SQL Server connections.

* `Korzh.DbUtils.MySQL`

  Implements DB manipulation interfaces for MySQL connections.
  
* `Korzh.DbUtils.PostgreSql`

  Implements DB manipulation interfaces for PosrgreSql connections.
  
* `Korzh.DbUtils.Sqlite`

  Implements DB manipulation interfaces for SQLite connections.
  
* `Korzh.DbUtils.EntityFrameworkCore.InMemory`

  Implements DB manipulation interfaces for EFCore In-Memory database (for testing purposes only).
  
Here you can find the [full API reference of the library](https://korzh.aistant.com/db-utils/api-reference).

## Basic scenario: Data seeding in your app

Let's suppose you have a "master copy" of some DB which you need to duplicate on user's machine on the first start of your application. It's quite usual situation when you need to distribute some demonstration project for your class library (like we do a lot of time during our work on [EasyQuery library](https://korzh.com/easyquery)) or it's local database for you desktop application which must be seeded with some default data.

Here is how you solve this task with our DbUtils tool set.

### Step 1: Export you "master" DB

Just install `dbtool` as it's described above, add a connection to you DB and then export all data from it to some folder:

```bash
dotnet tool install -g Korzh.DbTool

dbtool connections add MyMasterDb sqlserver "{the connection string to your DB}"

dbtool export MyMasterDb
```

### Step 2: Add data files to your project

After the previous step you will have a new folder like `MyMasterDb-20190720` with a bunch of JSON files in it (one for each table). Copy all these files to your project's folder to `App_Data\DbSeed` sub-folder.
Please note, that you will also need to add those files to your project manually for .NET Framework projects.

### Step 3: DB initialization code

Let's suppose we have a ASP.NET Core project and we need to seed our DB with the data on the first start. The database itself is created automatically with Entity Framework Core migrations. To seed it with the data we just need:

#### 1. Install Korzh.DbUtils NuGet packages                                                             

In this case we will need 2 of them:

* `Korzh.DbUtils.Import`

* `Korzh.DbUtils.SqlServer`

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

## Testing scenario: Data seeding in your unit and integration tests

### Step 1: Export you "master" DB (described in Basic Scenario)

### Step 2: Add data files to your project
After the previous step you will have a new folder like `MyMasterDb-20190720` with a bunch of JSON files in it (one for each table). In this example we will use `embedded resources`, but you can youse `zip` archive or files in folder as well. Copy all generated files to your test project's folder `Resources` and mark them as `Embedded resource`.
Please note, that you will also need to add those files to your project manually for .NET Framework projects.

### Step 3: DB initialization code

#### 1. Install Korzh.DbUtils NuGet packages

In this case we will need 2 of them:

* `Korzh.DbUtils.Import`

* `Korzh.DbUtils.EnityFrameworkCore.InMemory` (or `Korzh.DbUtils.Sqlite` )

#### 2. Add the initialization code

Here is an example of the such code we need to add for testing `AppDbContext`:
```c#
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseInMemoryDatabase("test-db");

var dbContext = new AppDbContext(optionsBuilder.Options);
dbContext.Database.EnsureCreated();

DbInitializer.Create(options => {
    options.UseInMemoryDatabase(dbContext);
    options.UseJsonImporter();
    options.UseResourceFileUnpacker(typeof(YourTestClass).Assembly, "Resources");
})
.Seed();

```
Here you can see [a full example](https://github.com/kedonec/Korzh.DbUtils/blob/09dfece6586f43845883826f47330de4a21e1101/tests/Korzh.DbUtils.EntityFrameworkCore.InMemory.Tests/DbContextBridgeTests.cs#L19).

If you would like to use in-memory SQLite database for testing, you can use such initialization code:
```c#
var connection = new SqliteConnection("Data Source=:memory:;");

// Create your test database here
...................................................................

// Seed data
DbInitializer.Create(options => {
    options.UseSqlite(connection);
    options.UseJsonImporter();
    options.UseResourceFileUnpacker(typeof(YourTestClass).Assembly, "Resources");
})
.Seed();
```
Here you can see [a full example](https://github.com/kedonec/Korzh.DbUtils/blob/4b809a9528958931eba2d28677103648fb1f5797/tests/Korzh.DbUtils.Sqlite.Tests/SqliteBridgeTests.cs#L17).
