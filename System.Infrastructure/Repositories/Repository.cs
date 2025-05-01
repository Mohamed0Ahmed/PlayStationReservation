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

        private IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool includeDeleted = false,
            bool onlyDeleted = false)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (onlyDeleted)
                query = query.Where(e => e.IsDeleted);

            else if (!includeDeleted)
                query = query.Where(e => !e.IsDeleted);


            if (predicate != null)
                query = query.Where(predicate);


            if (include != null)
                query = include(query);


            return query;
        }

        public async Task<T> GetByIdAsync(TKey id, bool includeDeleted = false, bool onlyDeleted = false)
        {
            return await GetQueryable(e => e.Id.Equals(id), null, includeDeleted, onlyDeleted)
                .FirstOrDefaultAsync() ?? null!;
        }

        public async Task<T> GetByIdWithIncludesAsync(TKey id, bool includeDeleted = false, bool onlyDeleted = false, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            return await GetQueryable(e => e.Id.Equals(id), include, includeDeleted, onlyDeleted)
                .FirstOrDefaultAsync() ?? null!;
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false, bool onlyDeleted = false)
        {
            return await GetQueryable(null, null, includeDeleted, onlyDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(bool includeDeleted = false, bool onlyDeleted = false, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            return await GetQueryable(null, include, includeDeleted, onlyDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, bool onlyDeleted = false)
        {
            return await GetQueryable(predicate, null, includeDeleted, onlyDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, bool onlyDeleted = false, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            return await GetQueryable(predicate, include, includeDeleted, onlyDeleted)
                .ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false, bool onlyDeleted = false)
        {
            return await GetQueryable(predicate, null, includeDeleted, onlyDeleted)
                .AnyAsync();
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
            var entity = await GetByIdAsync(id, includeDeleted: true, onlyDeleted: true);
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