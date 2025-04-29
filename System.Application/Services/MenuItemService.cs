using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuItemService(IUnitOfWork unitOfWork, IMenuCategoryService menuCategoryService)
        {
            _unitOfWork = unitOfWork;
            _menuCategoryService = menuCategoryService;
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(id);
            if (menuItem == null)
                throw new CustomException("MenuItem not found.", 404);
            return menuItem;
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId, bool includeDeleted = false)
        {
            var category = await _menuCategoryService.GetMenuCategoryByIdAsync(categoryId);
            return await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(mi => mi.MenuCategoryId == categoryId, includeDeleted);
        }

        public async Task AddMenuItemAsync(MenuItem menuItem)
        {
            if (string.IsNullOrWhiteSpace(menuItem.Name))
                throw new CustomException("Menu item name is required.", 400);
            if (menuItem.Price < 0)
                throw new CustomException("Price cannot be negative.", 400);
            if (menuItem.PointsRequired < 0)
                throw new CustomException("Points required cannot be negative.", 400);

            // Check for duplicate Name and MenuCategoryId
            var existingItem = (await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(mi => mi.Name == menuItem.Name && mi.MenuCategoryId == menuItem.MenuCategoryId && !mi.IsDeleted)).FirstOrDefault();
            if (existingItem != null)
                throw new CustomException($"A menu item with the name '{menuItem.Name}' already exists in this category.", 400);

            await _menuCategoryService.GetMenuCategoryByIdAsync(menuItem.MenuCategoryId);
            await _unitOfWork.GetRepository<MenuItem, int>().AddAsync(menuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMenuItemAsync(MenuItem menuItem)
        {
            var existingMenuItem = await GetMenuItemByIdAsync(menuItem.Id);
            if (string.IsNullOrWhiteSpace(menuItem.Name))
                throw new CustomException("Menu item name is required.", 400);
            if (menuItem.Price < 0)
                throw new CustomException("Price cannot be negative.", 400);
            if (menuItem.PointsRequired < 0)
                throw new CustomException("Points required cannot be negative.", 400);

            // Check for duplicate Name and MenuCategoryId (excluding the current item)
            var duplicateItem = (await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(mi => mi.Name == menuItem.Name && mi.MenuCategoryId == menuItem.MenuCategoryId && mi.Id != menuItem.Id && !mi.IsDeleted)).FirstOrDefault();
            if (duplicateItem != null)
                throw new CustomException($"Another menu item with the name '{menuItem.Name}' already exists in this category.", 400);

            await _menuCategoryService.GetMenuCategoryByIdAsync(menuItem.MenuCategoryId);
            existingMenuItem.Name = menuItem.Name;
            existingMenuItem.Price = menuItem.Price;
            existingMenuItem.PointsRequired = menuItem.PointsRequired;
            existingMenuItem.MenuCategoryId = menuItem.MenuCategoryId;
            _unitOfWork.GetRepository<MenuItem, int>().Update(existingMenuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var menuItem = await GetMenuItemByIdAsync(id);
            _unitOfWork.GetRepository<MenuItem, int>().Delete(menuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreMenuItemAsync(int id)
        {
            var menuItem = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(id, true);
            if (menuItem == null)
                throw new CustomException("MenuItem not found.", 404);
            if (!menuItem.IsDeleted)
                throw new CustomException("MenuItem is not deleted.", 400);

            await _unitOfWork.GetRepository<MenuItem, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}