using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<MenuItem, int> _itemRepository;

        public MenuService(
            IRepository<Category, int> categoryRepository,
            IRepository<MenuItem, int> itemRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
        }

        public async Task<ApiResponse<Category>> CreateCategoryAsync(string name, int storeId)
        {
            var category = new Category
            {
                Name = name,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            return new ApiResponse<Category>(category, "تم إضافة القسم بنجاح", 201);
        }

        public async Task<ApiResponse<Category>> UpdateCategoryAsync(int categoryId, string name)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new ApiResponse<Category>("القسم غير موجود", 404);
            }

            category.Name = name;
            category.LastModifiedOn = DateTime.UtcNow;
            _categoryRepository.Update(category);

            return new ApiResponse<Category>(category, "تم تعديل القسم بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new ApiResponse<bool>("القسم غير موجود", 404);
            }

            _categoryRepository.Delete(category);
            return new ApiResponse<bool>(true, "تم حذف القسم بنجاح");
        }

        public async Task<ApiResponse<List<Category>>> GetCategoriesAsync(int storeId)
        {
            var categories = await _categoryRepository.FindAsync(c => c.StoreId == storeId);
            return new ApiResponse<List<Category>>(categories.ToList());
        }

        public async Task<ApiResponse<MenuItem>> CreateItemAsync(string name, decimal price, int pointsRequired, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new ApiResponse<MenuItem>("القسم غير موجود", 404);
            }

            var item = new MenuItem
            {
                Name = name,
                Price = price,
                PointsRequired = pointsRequired,
                MenuCategoryId = categoryId,
                CreatedOn = DateTime.UtcNow
            };

            await _itemRepository.AddAsync(item);
            return new ApiResponse<MenuItem>(item, "تم إضافة الصنف بنجاح", 201);
        }

        public async Task<ApiResponse<MenuItem>> UpdateItemAsync(int itemId, string name, decimal price, int pointsRequired)
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                return new ApiResponse<MenuItem>("الصنف غير موجود", 404);
            }

            item.Name = name;
            item.Price = price;
            item.PointsRequired = pointsRequired;
            item.LastModifiedOn = DateTime.UtcNow;
            _itemRepository.Update(item);

            return new ApiResponse<MenuItem>(item, "تم تعديل الصنف بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteItemAsync(int itemId)
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                return new ApiResponse<bool>("الصنف غير موجود", 404);
            }

            _itemRepository.Delete(item);
            return new ApiResponse<bool>(true, "تم حذف الصنف بنجاح");
        }

        public async Task<ApiResponse<List<MenuItem>>> GetItemsAsync(int categoryId)
        {
            var items = await _itemRepository.FindWithIncludesAsync(
                predicate: mi => mi.MenuCategoryId == categoryId,
                include: q => q.Include(mi => mi.Category));

            return new ApiResponse<List<MenuItem>>(items.ToList());
        }

        public async Task<ApiResponse<int>> GetTotalItemsCountAsync(int storeId)
        {
            var count = (await _itemRepository.FindWithIncludesAsync(
                mi => mi.Category.StoreId == storeId,
                include: q => q.Include(mi => mi.Category))).Count();
            return new ApiResponse<int>(count, "تم جلب عدد الأصناف بنجاح");
        }
    }
}