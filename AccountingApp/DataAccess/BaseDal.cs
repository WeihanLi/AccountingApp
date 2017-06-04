﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AccountingApp.DataAccess
{
    public class BaseRepository<T> where T : Models.BaseModel
    {
        protected readonly Models.AccountingDbContext _dbEntity;

        public BaseRepository(Models.AccountingDbContext dbEntity)
        {
            _dbEntity = dbEntity;
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedTime = DateTime.Now;
            entity.UpdatedTime = DateTime.Now;
            _dbEntity.Set<T>().Add(entity);
            await _dbEntity.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// update(update all property)
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T entity)
        {
            entity.UpdatedTime = DateTime.Now;
            // reslove the exception
            // cannot be tracked because another instance of this type with the same key is already being tracked
            // https://stackoverflow.com/questions/6033638/an-object-with-the-same-key-already-exists-in-the-objectstatemanager-the-object
            _dbEntity.Entry(entity).CurrentValues.SetValues(entity);
            _dbEntity.Update(entity);
            return await _dbEntity.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(T entity, params string[] propertyNames)
        {
            entity.UpdatedTime = DateTime.Now;
            var entry = _dbEntity.Entry(entity);
            // reslove the exception
            // cannot be tracked because another instance of this type with the same key is already being tracked
            // https://stackoverflow.com/questions/6033638/an-object-with-the-same-key-already-exists-in-the-objectstatemanager-the-object
            entry.CurrentValues.SetValues(entity);

            entry.State = EntityState.Unchanged;
            foreach (string proName in propertyNames)
            {
                entry.Property(proName).IsModified = true;
            }
            return await _dbEntity.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync<TProperty>(T entity, params Expression<Func<T, TProperty>>[] propertyNames)
        {
            entity.UpdatedTime = DateTime.Now;
            var entry = _dbEntity.Entry(entity);
            // reslove the exception
            // cannot be tracked because another instance of this type with the same key is already being tracked
            // https://stackoverflow.com/questions/6033638/an-object-with-the-same-key-already-exists-in-the-objectstatemanager-the-object
            entry.CurrentValues.SetValues(entity);

            entry.State = EntityState.Unchanged;
            foreach (var proName in propertyNames)
            {
                entry.Property(proName).IsModified = true;
            }
            return await _dbEntity.SaveChangesAsync() > 0;
        }

        public async Task<List<T>> SelectAsync<TKey>(Expression<Func<T, bool>> whereLamdba, Expression<Func<T, TKey>> orderbyLambda, bool isAsc = false)
        {
            if (isAsc)
            {
                return await _dbEntity.Set<T>().AsNoTracking().Where(whereLamdba).OrderBy(orderbyLambda).ToListAsync();
            }
            else
            {
                return await _dbEntity.Set<T>().AsNoTracking().Where(whereLamdba).OrderByDescending(orderbyLambda).ToListAsync();
            }
        }

        public async Task<List<T>> SelectAsync<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLamdba, Expression<Func<T, TKey>> orderbyLambda, bool isAsc = false)
        {
            int offset = (pageIndex - 1) * pageSize;
            if (isAsc)
            {
                return await _dbEntity.Set<T>().AsNoTracking().Where(whereLamdba).Skip(offset).Take(pageSize).OrderBy(orderbyLambda).ToListAsync();
            }
            else
            {
                return await _dbEntity.Set<T>().AsNoTracking().Where(whereLamdba).Skip(offset).Take(pageSize).OrderByDescending(orderbyLambda).ToListAsync();
            }
        }

        public async Task<T> FetchAsync(int id)
        {
            return await _dbEntity.Set<T>().AsNoTracking().FirstOrDefaultAsync(e=>e.PKID == id);
        }

        public async Task<T> FetchAsync(Expression<Func<T, bool>> whereLamdba)
        {
            return await _dbEntity.Set<T>().AsNoTracking().FirstOrDefaultAsync(whereLamdba);
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> whereLamdba)
        {
            return await _dbEntity.Set<T>().AsNoTracking().AnyAsync(whereLamdba);
        }

        public async Task<int> QueryCountAsync(Expression<Func<T, bool>> whereLamdba)
        {
            return await _dbEntity.Set<T>().AsNoTracking().CountAsync(whereLamdba);
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereLamdba)
        {
            var list = _dbEntity.Set<T>().Where(whereLamdba).ToList();
            if (list != null && list.Any())
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].IsDeleted = true;
                }
                return await _dbEntity.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
