using AccountingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public async Task<List<Bill>> SelectWithTypeInfoAsync<TKey>(Expression<Func<Bill, bool>> whereLamdba, Expression<Func<Bill, TKey>> orderbyLambda, bool isAsc = false)
        {
            List<Bill> data = null;
            if (isAsc)
            {
                data = await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba).OrderBy(orderbyLambda).ToListAsync();
            }
            else
            {
                data = await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba).OrderByDescending(orderbyLambda).ToListAsync();
            }
            var types = _dbEntity.Set<BillType>().AsNoTracking().Where(t => !t.IsDeleted);
            foreach (var item in data)
            {
                item.AccountBillType = await types.FirstOrDefaultAsync(t => t.PKID == item.BillType);
            }
            return data;
        }

        public async Task<List<Bill>> SelectWithTypeInfoAsync<TKey>(int pageIndex, int pageSize, Expression<Func<Bill, bool>> whereLamdba, Expression<Func<Bill, TKey>> orderbyLambda, bool isAsc = false)
        {
            int offset = (pageIndex - 1) * pageSize;
            List<Bill> data = null;
            if (isAsc)
            {
                data =  await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba).Skip(offset).Take(pageSize).OrderBy(orderbyLambda).ToListAsync();
            }
            else
            {
                data =  await _dbEntity.Set<Bill>().AsNoTracking().Where(whereLamdba).Skip(offset).Take(pageSize).OrderByDescending(orderbyLambda).ToListAsync();
            }
            var types = _dbEntity.Set<BillType>().AsNoTracking().Where(t => !t.IsDeleted);
            foreach (var item in data)
            {
                item.AccountBillType = await types.FirstOrDefaultAsync(t => t.PKID == item.BillType);
            }
            return data;
        }
    }
}