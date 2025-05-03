using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface IMenuService
    {
        Task<ApiResponse<Category>> CreateCategoryAsync(string name, int storeId);
        Task<ApiResponse<Category>> UpdateCategoryAsync(int categoryId, string name);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId);
        Task<ApiResponse<bool>> RestoreCategoryAsync(int categoryId);
        Task<ApiResponse<List<Category>>> GetCategoriesAsync(int storeId);
        Task<ApiResponse<MenuItem>> CreateItemAsync(string name, decimal price, int pointsRequired, int categoryId);
        Task<ApiResponse<MenuItem>> UpdateItemAsync(int itemId, string name, decimal price, int pointsRequired);
        Task<ApiResponse<bool>> DeleteItemAsync(int itemId);
        Task<ApiResponse<bool>> RestoreItemAsync(int itemId);
        Task<ApiResponse<List<MenuItem>>> GetItemsAsync(int categoryId);
        Task<ApiResponse<int>> GetTotalItemsCountAsync(int storeId);
    }
}