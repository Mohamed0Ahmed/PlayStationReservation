using System.Linq.Expressions;
using System.Shared.BaseModel;

namespace System.Infrastructure.Repositories
{
    public interface IRepository<T, TKey> where T : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        Task<T> GetByIdAsync(TKey id, bool includeDeleted = false);
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task RestoreAsync(TKey id);
    }
}