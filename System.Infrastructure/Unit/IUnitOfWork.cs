using System.Domain;
using System.Infrastructure.Repositories;
using System.Shared.BaseModel;

namespace System.Infrastructure.Unit
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T, TKey> GetRepository<T, TKey>() where T : BaseEntity<TKey> where TKey : IEquatable<TKey>;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}