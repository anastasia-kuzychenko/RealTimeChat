using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RealTimeChat.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async ValueTask<List<TEntity>> Get()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async ValueTask<List<TEntity>> Get(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.Where(expression).AsNoTracking().ToListAsync();
        }

        public async ValueTask<TEntity> FindFirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.Where(expression).FirstOrDefaultAsync();
        }

        public async ValueTask<bool> Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return await Save();
        }

        public virtual async ValueTask<bool> Update(TEntity entity)
        {
            _dbSet.Update(entity);
            return await Save();
        }

        public virtual async ValueTask<bool> Update(IEnumerable<TEntity> entity)
        {
            _dbSet.UpdateRange(entity);
            return await Save();
        }

        public async ValueTask<bool> Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            return await Save();
        }

        protected async ValueTask<bool> Save()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
