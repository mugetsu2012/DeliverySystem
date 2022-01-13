using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEncomiendas.Core.Data
{
    public interface IGenericRepository<TEntity> where TEntity: class
    {
        IQueryable<TEntity> GetAll();

        Task<List<TEntity>> GetListaAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> GetByIdAsync(object id);

        Task<TEntity> GetByIdAsNoTracking(Expression<Func<TEntity, bool>> where);

        Task<TEntity> Create(TEntity entity);

        Task<List<TEntity>> Create(List<TEntity> entities);

        Task Update(object id, TEntity entity);

        Task Delete(object id);

        Task SaveChangesAsync();
    }
}
