using AccountingApp.Helper;
using AccountingApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using WeihanLi.Common;
using WeihanLi.DataProtection;
using WeihanLi.Npoi;

namespace AccountingApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration.ReplacePlaceholders();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add db service
            services.AddDbContext<Models.AccountingDbContext>(options => options.UseMySql(Configuration.GetConnectionString("AccountingConnection")));
            services.AddHealthChecks();
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
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddDataProtection()
                .AddParamsProtection(options =>
                {
                    options.ExpiresIn = 10;
                    options.AllowUnprotectedParams = true;
                    options.ProtectParams = new[]
                    {
                        "Id",
                        "pkid"
                    };
                    options.AddProtectValue<JsonResult>(r => r.Value);
                });

            //add AddAccessControlHelper
            services.AddAccessControlHelper<AccountingResourceAccessStrategy, AccountingControlAccessStrategy>();

            DependencyResolver.SetDependencyResolver(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // add
            loggerFactory.AddLog4Net();
            app.UseHealthChecks(new PathString("/health"));
            app.UseStaticFiles();

            // Add ASP.NET Core authentication
            app.UseAuthentication();
            // 权限控制
            // app.UseAccessControlHelper();

            app.UseMvcWithDefaultRoute();
            // FluentSettings for WeihanLi.Npoi
            FluentSettingForExcel();
        }

        private void FluentSettingForExcel()
        {
            var billSetting = ExcelHelper.SettingFor<Bill>();
            billSetting.HasTitle("账单统计");
            billSetting.Property(_ => _.BillTitle)
                .HasColumnTitle("账单标题")
                .HasColumnIndex(0);
            billSetting.Property(_ => _.BillType)
                .HasColumnTitle("账单类型")
                .HasColumnIndex(1);
            billSetting.Property(_ => _.BillFee)
                .HasColumnTitle("账单金额")
                .HasColumnIndex(2);
            billSetting.Property(_ => _.BillDescription)
                .HasColumnTitle("账单描述")
                .HasColumnIndex(3);
            billSetting.Property(_ => _.BillDetails)
                .HasColumnTitle("账单详情")
                .HasColumnIndex(4);
            billSetting.Property(_ => _.CreatedBy)
                .HasColumnTitle("创建人")
                .HasColumnIndex(5);
            billSetting.Property(_ => _.CreatedTime)
                .HasColumnTitle("创建时间")
                .HasColumnIndex(6)
                .HasColumnFormatter("yyyy-MM-dd HH:mm:ss");
            //
            billSetting.Property(_ => _.BillStatus).Ignored();
            billSetting.Property(_ => _.AccountBillType).Ignored();
            billSetting.Property(_ => _.PKID).Ignored();
            billSetting.Property(_ => _.UpdatedBy).Ignored();
            billSetting.Property(_ => _.UpdatedTime).Ignored();
            billSetting.Property(_ => _.IsDeleted).Ignored();
        }
    }
}
