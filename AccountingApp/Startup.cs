using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountingApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add db service
            // SqlServer
            //services.AddDbContext<Models.AccountingDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
            // MySql
            services.AddDbContext<Models.AccountingDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));

            //Cookie Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Account/Login";
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/LogOut";

                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });
            // Add framework services.
            services.AddMvc();

#if !DEBUG
            // enforce https
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
#endif

            //DbContext
            //reslove a exception on ef core,refer to https://github.com/aspnet/EntityFramework/issues/7762
            services.AddScoped<Models.AccountingDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Models.AccountingDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Add ASP.NET Core authentication
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
            // 权限控制
            app.UseAccessControlHelper(option =>
            {
                option.ActionAccessStrategy = new AccountingActionAccessStrategy();
                option.ControlAccessStrategy = new AccountingControlAccessStrategy();
            });
            // Initialize
            DataAccess.DbInitializer.Initialize(context);
        }
    }
}
