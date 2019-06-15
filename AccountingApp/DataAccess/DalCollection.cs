using AccountingApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AccountingApp.DataAccess
{
    public class DalBill : AccountingRepository<Bill>
    {
        public DalBill(AccountingDbContext dbEntity) : base(dbEntity)
        {
        }
    }

    public static class DalExtension
    {
        public static IServiceCollection AddAccountingRepository(this IServiceCollection services)
        {
            services.TryAddScoped<DalBill>();

            services.TryAddScoped(typeof(AccountingRepository<>));
            return services;
        }
    }
}
