using Mapster;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.Menu;

namespace System.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Categories

        //* Create Category
        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(string name, int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
                return new ApiResponse<CategoryDto>("No Store With This ID", 400);


            var existingCategory = await _unitOfWork.GetRepository<Category, int>().FindAsync(
                c => c.Name == name && c.StoreId == storeId);
            if (existingCategory.Any())
                return new ApiResponse<CategoryDto>("القسم موجود بالفعل", 400);


            var category = new Category
            {
                Name = name,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Category, int>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryDto = category.Adapt<CategoryDto>();
            return new ApiResponse<CategoryDto>(categoryDto, "تم إضافة القسم بنجاح", 201);
        }

        //* Update Category
        public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int categoryId, string name)
        {

            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<CategoryDto>("القسم غير موجود", 404);


            var existingCategory = await _unitOfWork.GetRepository<Category, int>().FindAsync(
                c => c.Name == name && c.StoreId == category.StoreId && c.Id != categoryId);
            if (existingCategory.Any())
                return new ApiResponse<CategoryDto>("اسم القسم موجود بالفعل", 400);


            category.Name = name;
            category.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Category, int>().Update(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryDto = category.Adapt<CategoryDto>();
            return new ApiResponse<CategoryDto>(categoryDto, "تم تعديل القسم بنجاح");
        }

        //* Delete Category
        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId)
        {

            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<bool>("القسم غير موجود", 404);

            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(i => i.CategoryId == categoryId);

            _unitOfWork.GetRepository<Category, int>().Delete(category);
            _unitOfWork.GetRepository<MenuItem, int>().DeleteRange(items);

            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف القسم بنجاح");
        }

        //* Restore Category
        public async Task<ApiResponse<bool>> RestoreCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId, onlyDeleted: true);
            if (category == null || !category.IsDeleted)
                return new ApiResponse<bool>("القسم غير موجود أو غير محذوف", 404);

            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(i => i.CategoryId == categoryId);


            await _unitOfWork.GetRepository<Category, int>().RestoreAsync(categoryId);
            _unitOfWork.GetRepository<MenuItem, int>().RestoreRange(items);

            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع القسم بنجاح");
        }

        //* Get Categories
        public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
                return new ApiResponse<List<CategoryDto>>("No Store To Get Categories", 404);


            var categories = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.StoreId == storeId);
            if (!categories.Any())
                return new ApiResponse<List<CategoryDto>>("لا يوجد أقسام", 404);

            var categoriesDto = categories.Adapt<List<CategoryDto>>();
            return new ApiResponse<List<CategoryDto>>(categoriesDto, "تم جلب الأقسام بنجاح");
        }

        #endregion

        #region Menu Items

        //* Create Item
        public async Task<ApiResponse<ItemDto>> CreateItemAsync(string name, decimal price, int pointsRequired, int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<ItemDto>("القسم غير موجود", 404);


            var existingItem = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.Name == name && mi.CategoryId == categoryId);
            if (existingItem.Any())
                return new ApiResponse<ItemDto>("الصنف موجود بالفعل", 400);


            var item = new MenuItem
            {
                Name = name,
                Price = price,
                PointsRequired = pointsRequired,
                CategoryId = categoryId,
                StoreId = category.StoreId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<MenuItem, int>().AddAsync(item);
            await _unitOfWork.SaveChangesAsync();

            var itemDto = item.Adapt<ItemDto>();
            return new ApiResponse<ItemDto>(itemDto, "تم إضافة الصنف بنجاح", 201);
        }

        //* Update Item
        public async Task<ApiResponse<ItemDto>> UpdateItemAsync(int itemId, string name, decimal price, int pointsRequired)
        {
            if (itemId <= 0 || string.IsNullOrEmpty(name) || price <= 0 || pointsRequired < 0)
                return new ApiResponse<ItemDto>(" اسم الصنف، السعر، والنقاط المطلوبة يجب أن يكونوا صالحين", 400);


            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId);
            if (item == null)
                return new ApiResponse<ItemDto>("الصنف غير موجود", 404);


            var existingItem = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.Name == name && mi.CategoryId == item.CategoryId && mi.Id != itemId);
            if (existingItem.Any())
                return new ApiResponse<ItemDto>("اسم الصنف موجود بالفعل", 400);


            item.Name = name;
            item.Price = price;
            item.PointsRequired = pointsRequired;
            item.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<MenuItem, int>().Update(item);
            await _unitOfWork.SaveChangesAsync();
            var itemDto = item.Adapt<ItemDto>();
            return new ApiResponse<ItemDto>(itemDto, "تم تعديل الصنف بنجاح");
        }

        //* Delete Item
        public async Task<ApiResponse<bool>> DeleteItemAsync(int itemId)
        {
            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId);
            if (item == null)
                return new ApiResponse<bool>("الصنف غير موجود", 404);


            _unitOfWork.GetRepository<MenuItem, int>().Delete(item);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف الصنف بنجاح");
        }

        //* Restore Item
        public async Task<ApiResponse<bool>> RestoreItemAsync(int itemId)
        {
            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId, onlyDeleted: true);
            if (item == null || !item.IsDeleted)
                return new ApiResponse<bool>("الصنف غير موجود أو غير محذوف", 404);


            await _unitOfWork.GetRepository<MenuItem, int>().RestoreAsync(itemId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع الصنف بنجاح");
        }

        //* Get Items
        public async Task<ApiResponse<List<ItemDto>>> GetItemsAsync(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.Id == categoryId);
            if (!category.Any())
                return new ApiResponse<List<ItemDto>>("هذا القسم غير موجود", 404);


            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(i => i.CategoryId == categoryId);

            if (!items.Any())
                return new ApiResponse<List<ItemDto>>("لا يوجد أصناف", 404);

            var itemsDto = items.Adapt<List<ItemDto>>();
            return new ApiResponse<List<ItemDto>>(itemsDto, "تم جلب الأصناف بنجاح");
        }

        //* Get Total Items Count
        public async Task<ApiResponse<List<ItemDto>>> GetAllItemsAsync(int storeId)
        {
            var items = (await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.StoreId == storeId)).Count();

            var itemsDto = items.Adapt<List<ItemDto>>();
            return new ApiResponse<List<ItemDto>>(itemsDto, "تم جلب كل الأصناف بنجاح");
        }

        #endregion
    }
}