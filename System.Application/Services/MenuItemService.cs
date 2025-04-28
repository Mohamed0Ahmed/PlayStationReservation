using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(id);
            if (menuItem == null)
                throw new Exception("MenuItem not found.");
            return menuItem;
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(mi => mi.MenuCategoryId == categoryId, includeDeleted);
        }

        public async Task AddMenuItemAsync(MenuItem menuItem)
        {
            await _unitOfWork.GetRepository<MenuItem, int>().AddAsync(menuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMenuItemAsync(MenuItem menuItem)
        {
            var existingMenuItem = await GetMenuItemByIdAsync(menuItem.Id);
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
            await _unitOfWork.GetRepository<MenuItem, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}