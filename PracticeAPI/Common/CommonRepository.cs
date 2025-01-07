
using Microsoft.EntityFrameworkCore;
using PracticeAPI.Context;
using System.Linq;
using System.Linq.Expressions;

namespace PracticeAPI.Common
{
    public class CommonRepository<T> : ICommonRepository<T> where T : class
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public CommonRepository(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task<T> Create(T record)
        {
            await _dbSet.AddAsync(record);
            await _dbContext.SaveChangesAsync();
            return record;
        }

        public async Task<bool> DeleteAsync(T record)
        {
            _dbSet.Remove(record);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByAnyFilter(Expression<Func<T,bool>> filter, bool isTracking)
        {
            if (isTracking)
            {
                var res = await _dbSet.Where(filter).AsNoTracking().FirstOrDefaultAsync();
                if (res == null)
                    return null;
                return res;
            }
            else
            {
                var res = await _dbSet.Where(filter).FirstOrDefaultAsync();
                if (res == null)
                    return null;
                return res;
            }
        }

        public async Task<List<T>> GetAllByFilterAsync(Expression<Func<T,bool>> filter,bool IsTracking)
        {
            if (IsTracking) {
                return await _dbSet.Where(filter).AsNoTracking().ToListAsync();
            }
            else
            {
                return await _dbSet.Where(filter).ToListAsync();
            }
        }

        public async Task<T> UpdateAsync(T record)
        {
            _dbContext.Update(record);
            await _dbContext.SaveChangesAsync();

            return record;
        }
    }
}
