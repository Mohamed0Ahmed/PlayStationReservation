using System.Linq.Expressions;
using System.Shared.BaseModel;

namespace System.Infrastructure.Repositories
{
    public interface IRepository<T, TKey> where T : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        Task<T> GetByIdAsync(TKey id, bool includeDeleted = false);
        Task<T> GetByIdWithIncludesAsync(TKey id, bool includeDeleted = false, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(bool includeDeleted = false, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false);
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task RestoreAsync(TKey id);
    }
}