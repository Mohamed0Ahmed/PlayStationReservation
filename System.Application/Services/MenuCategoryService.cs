using System.Infrastructure.Repositories;
using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MenuCategory> GetMenuCategoryByIdAsync(int id)
        {
            var menuCategory = await _unitOfWork.GetRepository<MenuCategory, int>().GetByIdAsync(id);
            if (menuCategory == null)
                throw new Exception("MenuCategory not found.");
            return menuCategory;
        }

        public async Task<IEnumerable<MenuCategory>> GetMenuCategoriesByStoreAsync(int storeId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<MenuCategory, int>().FindAsync(mc => mc.StoreId == storeId, includeDeleted);
        }

        public async Task AddMenuCategoryAsync(MenuCategory menuCategory)
        {
            await _unitOfWork.GetRepository<MenuCategory, int>().AddAsync(menuCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMenuCategoryAsync(MenuCategory menuCategory)
        {
            var existingMenuCategory = await GetMenuCategoryByIdAsync(menuCategory.Id);
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
            await _unitOfWork.GetRepository<MenuCategory, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}