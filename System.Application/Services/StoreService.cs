using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Store> GetStoreByIdAsync(int id)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(id);
            if (store == null)
                throw new Exception("Store not found.");
            return store;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync(bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<Store, int>().GetAllAsync(includeDeleted);
        }

        public async Task AddStoreAsync(Store store)
        {
            await _unitOfWork.GetRepository<Store, int>().AddAsync(store);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStoreAsync(Store store)
        {
            var existingStore = await GetStoreByIdAsync(store.Id);
            existingStore.Name = store.Name;
            existingStore.OwnerEmail = store.OwnerEmail;
            _unitOfWork.GetRepository<Store, int>().Update(existingStore);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteStoreAsync(int id)
        {
            var store = await GetStoreByIdAsync(id);
            _unitOfWork.GetRepository<Store, int>().Delete(store);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreStoreAsync(int id)
        {
            await _unitOfWork.GetRepository<Store, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}