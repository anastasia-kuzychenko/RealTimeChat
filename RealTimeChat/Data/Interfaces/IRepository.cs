using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RealTimeChat.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<List<TEntity>> Get();
        ValueTask<List<TEntity>> Get(Expression<Func<TEntity, bool>> expression);
        ValueTask<TEntity> FindFirstOrDefault(Expression<Func<TEntity, bool>> expression);
        ValueTask<bool> Create(TEntity entity);
        ValueTask<bool> Create(IEnumerable<TEntity> entity);
        ValueTask<bool> Update(TEntity entity);
        ValueTask<bool> Update(IEnumerable<TEntity> entity);
        ValueTask<bool> Delete(TEntity entity);
    }
}
