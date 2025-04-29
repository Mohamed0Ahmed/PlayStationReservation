using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
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

        public async Task<Store> GetStoreByIdAsync(int id, bool includeDeleted = false)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdWithIncludesAsync(id, includeDeleted, s => s.Rooms);
            if (store == null)
                throw new CustomException("Store not found.", 404);
            return store;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync(bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<Store, int>().GetAllWithIncludesAsync(includeDeleted, s => s.Rooms);
        }

        public async Task AddStoreAsync(Store store)
        {
            if (string.IsNullOrWhiteSpace(store.Name))
                throw new CustomException("Store name is required.", 400);
            if (string.IsNullOrWhiteSpace(store.OwnerEmail))
                throw new CustomException("Owner email is required.", 400);

            var existingStore = (await _unitOfWork.GetRepository<Store, int>().FindAsync(s => s.OwnerEmail == store.OwnerEmail && !s.IsDeleted)).FirstOrDefault();
            if (existingStore != null)
                throw new CustomException("A store with this owner email already exists.", 400);

            await _unitOfWork.GetRepository<Store, int>().AddAsync(store);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStoreAsync(Store store)
        {
            var existingStore = await GetStoreByIdAsync(store.Id);
            if (string.IsNullOrWhiteSpace(store.Name))
                throw new CustomException("Store name is required.", 400);
            if (string.IsNullOrWhiteSpace(store.OwnerEmail))
                throw new CustomException("Owner email is required.", 400);

            var duplicateStore = (await _unitOfWork.GetRepository<Store, int>().FindAsync(s => s.OwnerEmail == store.OwnerEmail && s.Id != store.Id && !s.IsDeleted)).FirstOrDefault();
            if (duplicateStore != null)
                throw new CustomException("Another store with this owner email already exists.", 400);

            existingStore.Name = store.Name;
            existingStore.OwnerEmail = store.OwnerEmail;
            _unitOfWork.GetRepository<Store, int>().Update(existingStore);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteStoreAsync(int id)
        {
            var store = await GetStoreByIdAsync(id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Soft delete all related Rooms
                if (store.Rooms != null)
                {
                    foreach (var room in store.Rooms.Where(r => !r.IsDeleted))
                    {
                        _unitOfWork.GetRepository<Room, int>().Delete(room);
                    }
                }

                _unitOfWork.GetRepository<Store, int>().Delete(store);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException($"Failed to delete store: {ex.Message}", 500);
            }
        }

        public async Task RestoreStoreAsync(int id)
        {
            var store = await GetStoreByIdAsync(id, true);
            if (!store.IsDeleted)
                throw new CustomException("Store is not deleted.", 400);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Restore all related Rooms
                if (store.Rooms != null)
                {
                    foreach (var room in store.Rooms.Where(r => r.IsDeleted))
                    {
                        await _unitOfWork.GetRepository<Room, int>().RestoreAsync(room.Id);
                    }
                }

                await _unitOfWork.GetRepository<Store, int>().RestoreAsync(id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to restore store.", 500);
            }
        }
    }
}