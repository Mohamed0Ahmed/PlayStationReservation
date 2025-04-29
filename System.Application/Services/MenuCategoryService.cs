using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreService _storeService;

        public MenuCategoryService(IUnitOfWork unitOfWork, IStoreService storeService)
        {
            _unitOfWork = unitOfWork;
            _storeService = storeService;
        }

        public async Task<MenuCategory> GetMenuCategoryByIdAsync(int id)
        {
            var menuCategory = await _unitOfWork.GetRepository<MenuCategory, int>().GetByIdWithIncludesAsync(id, false, mc => mc.MenuItems);
            if (menuCategory == null)
                throw new CustomException("MenuCategory not found.", 404);
            return menuCategory;
        }

        public async Task<IEnumerable<MenuCategory>> GetMenuCategoriesByStoreAsync(int storeId, bool includeDeleted = false)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId, includeDeleted);
            return await _unitOfWork.GetRepository<MenuCategory, int>().FindWithIncludesAsync(mc => mc.StoreId == storeId, includeDeleted, mc => mc.MenuItems);
        }

        public async Task AddMenuCategoryAsync(MenuCategory menuCategory)
        {
            if (string.IsNullOrWhiteSpace(menuCategory.Name))
                throw new CustomException("Menu category name is required.", 400);

            // Check for duplicate Name and StoreId
            var existingCategory = (await _unitOfWork.GetRepository<MenuCategory, int>().FindAsync(mc => mc.Name == menuCategory.Name && mc.StoreId == menuCategory.StoreId && !mc.IsDeleted)).FirstOrDefault();
            if (existingCategory != null)
                throw new CustomException($"A menu category with the name '{menuCategory.Name}' already exists for this store.", 400);

            await _storeService.GetStoreByIdAsync(menuCategory.StoreId);
            await _unitOfWork.GetRepository<MenuCategory, int>().AddAsync(menuCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMenuCategoryAsync(MenuCategory menuCategory)
        {
            var existingMenuCategory = await GetMenuCategoryByIdAsync(menuCategory.Id);
            if (string.IsNullOrWhiteSpace(menuCategory.Name))
                throw new CustomException("Menu category name is required.", 400);

            // Check for duplicate Name and StoreId (excluding the current category)
            var duplicateCategory = (await _unitOfWork.GetRepository<MenuCategory, int>().FindAsync(mc => mc.Name == menuCategory.Name && mc.StoreId == menuCategory.StoreId && mc.Id != menuCategory.Id && !mc.IsDeleted)).FirstOrDefault();
            if (duplicateCategory != null)
                throw new CustomException($"Another menu category with the name '{menuCategory.Name}' already exists for this store.", 400);

            await _storeService.GetStoreByIdAsync(menuCategory.StoreId);
            existingMenuCategory.Name = menuCategory.Name;
            existingMenuCategory.StoreId = menuCategory.StoreId;
            _unitOfWork.GetRepository<MenuCategory, int>().Update(existingMenuCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteMenuCategoryAsync(int id)
        {
            var menuCategory = await GetMenuCategoryByIdAsync(id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Soft delete all related MenuItems
                var menuItems = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(mi => mi.MenuCategoryId == id && !mi.IsDeleted);
                foreach (var menuItem in menuItems)
                {
                    _unitOfWork.GetRepository<MenuItem, int>().Delete(menuItem);
                }

                _unitOfWork.GetRepository<MenuCategory, int>().Delete(menuCategory);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to delete menu category.", 500);
            }
        }

        public async Task RestoreMenuCategoryAsync(int id)
        {
            var menuCategory = await _unitOfWork.GetRepository<MenuCategory, int>().GetByIdAsync(id, true);
            if (menuCategory == null)
                throw new CustomException("MenuCategory not found.", 404);
            if (!menuCategory.IsDeleted)
                throw new CustomException("MenuCategory is not deleted.", 400);

            await _unitOfWork.GetRepository<MenuCategory, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}