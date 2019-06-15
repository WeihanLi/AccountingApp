using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Models
{
    public class AccountingDbContext : DbContext
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="options"></param>
        public AccountingDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// init model settings
        /// </summary>
        /// <param name="modelBuilder">modelBuilder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TableName
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<BillType>().ToTable("BillTypes");
            modelBuilder.Entity<Bill>().ToTable("Bills");
            modelBuilder.Entity<BillPayItem>().ToTable("BillPayItems");

            //Primary Key
            modelBuilder.Entity<User>().HasKey(m => m.PKID);
            modelBuilder.Entity<BillType>().HasKey(m => m.PKID);
            modelBuilder.Entity<Bill>().HasKey(m => m.PKID);
            modelBuilder.Entity<BillPayItem>().HasKey(m => m.PKID);

            //Identity
            modelBuilder.Entity<User>().Property(m => m.PKID).ValueGeneratedOnAdd();
            modelBuilder.Entity<BillType>().Property(m => m.PKID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Bill>().Property(m => m.PKID).ValueGeneratedOnAdd();
            modelBuilder.Entity<BillPayItem>().Property(m => m.PKID).ValueGeneratedOnAdd();

            // QueryFilter
            modelBuilder.Entity<User>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<BillType>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Bill>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<BillPayItem>().HasQueryFilter(m => !m.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }

        #region Tables

        /// <summary>
        /// Users
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 账单类型
        /// </summary>
        public DbSet<BillType> BillTypes { get; set; }

        /// <summary>
        /// 账单
        /// </summary>
        public DbSet<Bill> Bills { get; set; }

        /// <summary>
        /// 账单支付详情
        /// </summary>
        public DbSet<BillPayItem> BillPayItems { get; set; }

        #endregion Tables
    }
}
