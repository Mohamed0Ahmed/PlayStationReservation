using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Shared.BaseModel;
using System.Infrastructure.Repositories;

namespace System.Infrastructure.Unit
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed = false;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T, TKey> GetRepository<T, TKey>() where T : BaseEntity<TKey> where TKey : IEquatable<TKey>
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T, TKey>(_context);
            }
            return (IRepository<T, TKey>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
        
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to commit.");
            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null!;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to rollback.");
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null!;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_transaction != null)
                        {
                            _transaction.Dispose();
                            _transaction = null!;
                        }
                        _context.Dispose();
                    }
                    catch (Exception)
                    {
                        
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}