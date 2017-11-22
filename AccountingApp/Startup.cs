﻿using AccountingApp.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeihanLi.Common;

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
            // requires all requests use HTTPS
            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            {
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.RequireHttpsAttribute());
            });
#endif

            //add AddAccessControlHelper
            services.AddAccessControlHelper<AccountingActionAccessStrategy,AccountingControlAccessStrategy>();

            //DbContext
            //reslove a exception on ef core,refer to https://github.com/aspnet/EntityFramework/issues/7762
            services.AddScoped<Models.AccountingDbContext>();

            DependencyResolver.SetDependencyResolver(new MicrosoftExtensionDependencyResolver(services.BuildServiceProvider()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Models.AccountingDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            // add log4net
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Add ASP.NET Core authentication
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
#if !DEBUG
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl#require-ssl
            // redirects all HTTP requests to HTTPS
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
           
#endif
            // 权限控制
            app.UseAccessControlHelper();
            // Initialize
            DataAccess.DbInitializer.Initialize(context);
        }
    }
}