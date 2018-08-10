using System.IO;
using AccountingApp.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            // Initialize
            using (var scope = host.Services.CreateScope())
            {
                DataAccess.DbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<AccountingDbContext>());
            }

            host.Run();
        }
    }
}
