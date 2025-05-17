using System.Shared;
using System.Shared.DTOs.Menu;

namespace System.Application.Abstraction
{
    public interface IMenuService
    {
        Task<ApiResponse<CategoryDto>> CreateCategoryAsync(string name, int storeId);
        Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int categoryId, string name);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId);
        Task<ApiResponse<bool>> RestoreCategoryAsync(int categoryId);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync(int storeId);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetDeletedCategoriesAsync(int storeId);
        Task<ApiResponse<ItemDto>> CreateItemAsync(string name, decimal price, int categoryId);
        Task<ApiResponse<ItemDto>> UpdateItemAsync(int itemId, string name, decimal price);
        Task<ApiResponse<bool>> DeleteItemAsync(int itemId);
        Task<ApiResponse<bool>> DeleteHardItemAsync(int itemId);
        Task<ApiResponse<bool>> RestoreItemAsync(int itemId);
        Task<ApiResponse<IEnumerable<ItemDto>>> GetItemsAsync(int categoryId);
        Task<ApiResponse<IEnumerable<ItemDto>>> GetDeletedItemsAsync(int categoryId);
        Task<ApiResponse<IEnumerable<ItemDto>>> GetAllItemsAsync(int storeId);
    }
}