using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AccountingApp.Models;
using Microsoft.EntityFrameworkCore;
using WeihanLi.Extensions;

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

    public class DalBillPayItem : BaseRepository<BillPayItem>
    {
        public DalBillPayItem(AccountingDbContext dbEntity) : base(dbEntity)
        {
        }
    }

    public class DalBill : BaseRepository<Bill>
    {
        public DalBill(AccountingDbContext dbEntity) : base(dbEntity)
        {
        }

        public async Task<List<Bill>> SelectWithTypeInfoAsync<TKey>(Expression<Func<Bill, bool>> whereLamdba, Expression<Func<Bill, TKey>> orderbyLambda, bool isAsc = false)
        {
            List<Bill> data = null;
            if (isAsc)
            {
                data = await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba.And(t => !t.IsDeleted)).OrderBy(orderbyLambda).ToListAsync();
            }
            else
            {
                data = await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba.And(t => !t.IsDeleted)).OrderByDescending(orderbyLambda).ToListAsync();
            }
            var types = await _dbEntity.Set<BillType>().AsNoTracking().ToArrayAsync();
            foreach (var item in data)
            {
                item.AccountBillType = types.FirstOrDefault(t => t.PKID == item.BillType);
            }
            return data;
        }

        public async Task<List<Bill>> SelectWithTypeInfoAsync<TKey>(int pageIndex, int pageSize, Expression<Func<Bill, bool>> whereLamdba, Expression<Func<Bill, TKey>> orderbyLambda, bool isAsc = false)
        {
            int offset = (pageIndex - 1) * pageSize;
            List<Bill> data = null;
            if (isAsc)
            {
                data = await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba.And(t => !t.IsDeleted)).OrderBy(orderbyLambda).Skip(offset).Take(pageSize).ToListAsync();
            }
            else
            {
                data = await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba.And(t => !t.IsDeleted)).OrderByDescending(orderbyLambda).Skip(offset).Take(pageSize).ToListAsync();
            }
            var types = await _dbEntity.Set<BillType>().AsNoTracking().ToArrayAsync();
            foreach (var item in data)
            {
                item.AccountBillType = types.FirstOrDefault(t => t.PKID == item.BillType);
            }
            return data;
        }
    }
}
