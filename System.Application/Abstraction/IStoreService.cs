using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IStoreService
    {
        Task<Store> GetStoreByIdAsync(int id, bool includeDeleted = false);
        Task<Store> GetStoreByNameAsync(string name, bool includeDeleted = false);
        Task<Store> GetStoreByOwnerEmailAsync(string ownerEmail, bool includeDeleted = false);
        Task<IEnumerable<Store>> GetAllStoresAsync(bool includeDeleted = false);
        Task AddStoreAsync(Store store);
        Task UpdateStoreAsync(Store store);
        Task DeleteStoreAsync(int id);
        Task RestoreStoreAsync(int id);
    }
}