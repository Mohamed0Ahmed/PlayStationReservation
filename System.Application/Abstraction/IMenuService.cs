using System.Domain.Models;
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
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync(int storeId);
        Task<ApiResponse<ItemDto>> CreateItemAsync(string name, decimal price, int pointsRequired, int categoryId);
        Task<ApiResponse<ItemDto>> UpdateItemAsync(int itemId, string name, decimal price, int pointsRequired);
        Task<ApiResponse<bool>> DeleteItemAsync(int itemId);
        Task<ApiResponse<bool>> RestoreItemAsync(int itemId);
        Task<ApiResponse<List<ItemDto>>> GetItemsAsync(int categoryId);
        Task<ApiResponse<List<ItemDto>>> GetAllItemsAsync(int storeId);
    }
}