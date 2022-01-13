using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaEncomiendas.Core.Data;

namespace SistemaEncomiendas.Data
{
    public class GenericRepository<TEntity>: IGenericRepository<TEntity> where TEntity: class
    {
        private readonly DbContext _db;

        public GenericRepository(DbContext context)
        {
            _db = context;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _db.Set<TEntity>().AsNoTracking();
        }

        public async Task<List<TEntity>> GetListaAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _db.Set<TEntity>().AsNoTracking().Where(where).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _db.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            await _db.Set<TEntity>().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<List<TEntity>> Create(List<TEntity> entities)
        {
            await _db.Set<TEntity>().AddRangeAsync(entities);
            await _db.SaveChangesAsync();
            return entities;
        }

        public async Task Update(object id, TEntity entity)
        {
            _db.Set<TEntity>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(object id)
        {
            var entity = await GetByIdAsync(id);
            _db.Set<TEntity>().Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<TEntity> GetByIdAsNoTracking(Expression<Func<TEntity, bool>> where)
        {
            return await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(where);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
