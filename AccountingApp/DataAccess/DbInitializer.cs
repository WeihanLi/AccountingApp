using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApp.DataAccess
{
    public static class DbInitializer
    {
        public static void Initialize(Models.AccountingDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
            Models.User[] users = {
                new Models.User { Username = "liweihan",PasswordHash = Helper.SecurityHelper.SHA256_Encrypt("Test1234"),IsActive=true,CreatedBy = "System",CreatedTime = DateTime.Now,UpdatedBy = "System",UpdatedTime=DateTime.Now },
                new Models.User { Username = "heyafei",PasswordHash = Helper.SecurityHelper.SHA256_Encrypt("Test1234"),IsActive=true,CreatedBy = "System",CreatedTime = DateTime.Now,UpdatedBy = "System",UpdatedTime=DateTime.Now },
                new Models.User { Username = "zoushirong",PasswordHash = Helper.SecurityHelper.SHA256_Encrypt("Test1234"),IsActive=true,CreatedBy = "System",CreatedTime = DateTime.Now,UpdatedBy = "System",UpdatedTime=DateTime.Now }
            };
            context.Users.AddRange(users);
            Models.BillType[] types =
            {
                new Models.BillType{ TypeName = "房租费用", TypeDesc = "房租及自如服务费",CreatedBy = "System",CreatedTime = DateTime.Now,UpdatedBy = "System",UpdatedTime=DateTime.Now },
                new Models.BillType{ TypeName = "生活费用", TypeDesc = "水电燃气费等",CreatedBy = "System",CreatedTime = DateTime.Now,UpdatedBy = "System",UpdatedTime=DateTime.Now },
                new Models.BillType{ TypeName = "家庭费用", TypeDesc = "家用设施家用电器等",CreatedBy = "System",CreatedTime = DateTime.Now,UpdatedBy = "System",UpdatedTime=DateTime.Now },
            };
            context.AddRange(types);
            context.SaveChanges();
        }
    }
}
