using Microsoft.EntityFrameworkCore;
using System.Infrastructure.Data;
using System.Linq.Expressions;
using System.Shared.BaseModel;

namespace System.Infrastructure.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(TKey id, bool includeDeleted = false)
        {
            var query = _dbSet.AsQueryable();
            if (!includeDeleted)
                query = query.Where(e => !e.IsDeleted);
            return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _dbSet.AsQueryable();
            if (!includeDeleted)
                query = query.Where(e => !e.IsDeleted);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            var query = _dbSet.AsQueryable();
            if (!includeDeleted)
                query = query.Where(e => !e.IsDeleted);
            return await query.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            entity.LastModifiedOn = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public async Task RestoreAsync(TKey id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id));
            if (entity != null)
            {
                entity.IsDeleted = false;
                entity.DeletedOn = null;
                entity.LastModifiedOn = DateTime.UtcNow;
                _dbSet.Update(entity);
            }
        }
    }
}