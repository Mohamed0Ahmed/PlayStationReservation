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
            var menuCategory = await _unitOfWork.GetRepository<MenuCategory, int>().GetByIdAsync(id);
            if (menuCategory == null)
                throw new CustomException("MenuCategory not found.", 404);
            return menuCategory;
        }

        public async Task<IEnumerable<MenuCategory>> GetMenuCategoriesByStoreAsync(int storeId, bool includeDeleted = false)
        {
            await _storeService.GetStoreByIdAsync(storeId);
            return await _unitOfWork.GetRepository<MenuCategory, int>().FindAsync(mc => mc.StoreId == storeId, includeDeleted);
        }

        public async Task AddMenuCategoryAsync(MenuCategory menuCategory)
        {
            if (string.IsNullOrWhiteSpace(menuCategory.Name))
                throw new CustomException("Menu category name is required.", 400);

            await _storeService.GetStoreByIdAsync(menuCategory.StoreId);
            await _unitOfWork.GetRepository<MenuCategory, int>().AddAsync(menuCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMenuCategoryAsync(MenuCategory menuCategory)
        {
            var existingMenuCategory = await GetMenuCategoryByIdAsync(menuCategory.Id);
            if (string.IsNullOrWhiteSpace(menuCategory.Name))
                throw new CustomException("Menu category name is required.", 400);

            await _storeService.GetStoreByIdAsync(menuCategory.StoreId); 
            existingMenuCategory.Name = menuCategory.Name;
            existingMenuCategory.StoreId = menuCategory.StoreId;
            _unitOfWork.GetRepository<MenuCategory, int>().Update(existingMenuCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteMenuCategoryAsync(int id)
        {
            var menuCategory = await GetMenuCategoryByIdAsync(id);
            _unitOfWork.GetRepository<MenuCategory, int>().Delete(menuCategory);
            await _unitOfWork.SaveChangesAsync();
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