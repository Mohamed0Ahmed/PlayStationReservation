using System.Linq.Expressions;
using System.Shared.BaseModel;

namespace System.Infrastructure.Repositories
{
    public interface IRepository<T, TKey> where T : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        Task<T> GetByIdAsync(TKey id, bool includeDeleted = false, bool onlyDeleted = false);
        Task<T> GetByIdWithIncludesAsync(TKey id, bool includeDeleted = false, bool onlyDeleted = false, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false, bool onlyDeleted = false);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(bool includeDeleted = false, bool onlyDeleted = false, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, bool onlyDeleted = false);
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, bool onlyDeleted = false, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false, bool onlyDeleted = false);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);            
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);            
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);             
        Task RestoreAsync(TKey id);
        void RestoreRange(IEnumerable<T> entities);

    }

}