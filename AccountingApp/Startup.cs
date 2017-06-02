﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            services.AddDbContext<Models.AccountingDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //Authentication
            services.AddAuthentication();
            // Add framework services.
            services.AddMvc();
            //DbContext
            services.AddSingleton<Models.AccountingDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,Models.AccountingDbContext context)
        {
            // 解决中文乱码问题，引用 System.Text.Encoding.CodePages 
            // - http://www.cnblogs.com/wolf-sun/p/6136482.html
            // - https://msdn.microsoft.com/zh-cn/library/system.text.encoding.registerprovider(v=vs.110).aspx
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

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

            // Add ASP.NET Core Identity
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                LoginPath = "/Account/Login",
                LogoutPath = "/Account/LogOut",
                AccessDeniedPath = "/Account/Login",
                AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                CookieHttpOnly = true,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
            // Initialize
            DataAccess.DbInitializer.Initialize(context);
        }
    }
}
