using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Korzh.DbUtils.Import;
using Korzh.DbUtils.Packing;
using Korzh.DbUtils.SqlServer;
using Korzh.DbUtils.MySql;

namespace AspNetCoreTestApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => {
                //options.UseSqlServer(Configuration.GetConnectionString("EqDbDemo"));
                options.UseMySQL(Configuration.GetConnectionString("EqDbDemoMySQL"));  
            });

            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseMvc();

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider.GetService<AppDbContext>()) {
                if (context.Database.EnsureCreated()) {
                    Korzh.DbUtils.DbInitializer.Create(options => {
                        options.NeedDataSeeding = true;
                        //options.UseDbContext(dbContext);
                        //options.UseDbContext<AppDbContext>(app.ApplicationServices, false);  //options => options.UseSqlServer(Configuration.GetConnectionString("EqDbDemo"))
                        //options.UseSqlServer(Configuration.GetConnectionString("EqDbDemo"));
                        options.UseMySQL(Configuration.GetConnectionString("EqDbDemoMySQL"));
                        options.UseJsonImporter();
                        options.UseFileFolderPacker(System.IO.Path.Combine(env.ContentRootPath, "App_Data", "InitialData"));
                        //options.UseZipPacker(System.IO.Path.Combine(env.ContentRootPath, "App_Data", "dataseed.zip"));
                    })
                    .Run();
                }
            }
        }
    }
}
