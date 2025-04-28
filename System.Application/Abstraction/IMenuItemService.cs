using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface IMenuItemService
    {
        Task<MenuItem> GetMenuItemByIdAsync(int id);
        Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId, bool includeDeleted = false);
        Task AddMenuItemAsync(MenuItem menuItem);
        Task UpdateMenuItemAsync(MenuItem menuItem);
        Task DeleteMenuItemAsync(int id);
        Task RestoreMenuItemAsync(int id);
    }
}