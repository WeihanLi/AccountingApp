using AccountingApp.Models;

namespace AccountingApp.DataAccess
{
    public class DalUser : BaseRepository<User>
    {
        public DalUser(AccountingDbContext dbEntity) : base(dbEntity)
        {
        }
    }

    public class DalBillType : BaseRepository<BillType>
    {
        public DalBillType(AccountingDbContext dbEntity) : base(dbEntity)
        {
        }
    }

    public class DalBill : BaseRepository<Bill>
    {
        public DalBill(AccountingDbContext dbEntity) : base(dbEntity)
        {
        }
    }
}