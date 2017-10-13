using AccountingApp.Models;

namespace AccountingApp.Helper
{
    public class BusinessHelper
    {
        private readonly AccountingDbContext _dbContext;

        public BusinessHelper(AccountingDbContext dbEntity)
        {
            _dbContext = dbEntity;
        }

        private DataAccess.DalUser userHelper;

        public DataAccess.DalUser UserHelper
        {
            get
            {
                if (userHelper == null)
                {
                    userHelper = new DataAccess.DalUser(_dbContext);
                }
                return userHelper;
            }
        }

        private DataAccess.DalBillType billTypeHelper;

        public DataAccess.DalBillType BillTypeHelper
        {
            get
            {
                if (billTypeHelper == null)
                {
                    billTypeHelper = new DataAccess.DalBillType(_dbContext);
                }
                return billTypeHelper;
            }
        }

        private DataAccess.DalBill billHelper;

        public DataAccess.DalBill BillHelper
        {
            get
            {
                if (billHelper == null)
                {
                    billHelper = new DataAccess.DalBill(_dbContext);
                }
                return billHelper;
            }
        }

        private DataAccess.DalBillPayItem billPayItemHelper;

        public DataAccess.DalBillPayItem BillPayItemHelper
        {
            get
            {
                if (billPayItemHelper == null)
                {
                    billPayItemHelper = new DataAccess.DalBillPayItem(_dbContext);
                }
                return billPayItemHelper;
            }
        }
    }
}