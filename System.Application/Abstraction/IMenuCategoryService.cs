using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IMenuCategoryService
    {
        Task<MenuCategory> GetMenuCategoryByIdAsync(int id);
        Task<IEnumerable<MenuCategory>> GetMenuCategoriesByStoreAsync(int storeId, bool includeDeleted = false);
        Task AddMenuCategoryAsync(MenuCategory menuCategory);
        Task UpdateMenuCategoryAsync(MenuCategory menuCategory);
        Task DeleteMenuCategoryAsync(int id);
        Task RestoreMenuCategoryAsync(int id);
    }
}