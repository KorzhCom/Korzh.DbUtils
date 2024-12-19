using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Korzh.DbUtils;

namespace DbUtilsDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public readonly string DbConnectionString = "Data Source=test.db;";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => {
                //options.UseSqlServer(Configuration.GetConnectionString("DbUtilsDemoDb01"));
                //options.UseNpgsql(Configuration.GetConnectionString("DbUtilsDemoDb03"));
                //options.UseInMemoryDatabase("test-db");
                options.UseSqlite(DbConnectionString);
            });

            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
            });

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider.GetService<AppDbContext>()) {
                if (context.Database.EnsureCreated()) { //run initializer only for the newly created DB
                    DbInitializer.Create(options => {
                        options.UseSqlite(DbConnectionString);
                        //options.UseSqlServer(Configuration.GetConnectionString("DbUtilsDemoDb01"));
                        //options.UseMySQL(Configuration.GetConnectionString("DbUtilsDemoDb02"));
                        //options.UsePostgreSql(Configuration.GetConnectionString("DbUtilsDemoDb03"));
                        //options.UseInMemoryDatabase(context);
                        options.UseJsonImporter();
                        options.UseFileFolderPacker(System.IO.Path.Combine(env.ContentRootPath, "App_Data", "SeedData"));
                        //options.UseZipPacker(System.IO.Path.Combine(env.ContentRootPath, "App_Data", "dataseed.zip"));
                    }, scope.ServiceProvider.GetRequiredService<ILoggerFactory>())
                    .Seed();
                }
            }
        }
    }
}
