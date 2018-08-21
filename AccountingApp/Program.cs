using AccountingApp.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                    {
                        var builtConfig = config.Build();

                        config.AddAzureKeyVault(
                            $"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
                            builtConfig["KeyVault:ClientId"],
                            builtConfig["KeyVault:ClientSecret"]);
                    })
                .UseStartup<Startup>()
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
