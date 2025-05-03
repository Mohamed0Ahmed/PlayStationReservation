using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;

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
        public async Task<ApiResponse<Category>> CreateCategoryAsync(string name, int storeId)
        {

            var existingCategory = await _unitOfWork.GetRepository<Category, int>().FindAsync(
                c => c.Name == name && c.StoreId == storeId);
            if (existingCategory != null)
                return new ApiResponse<Category>("القسم موجود بالفعل", 400);


            var category = new Category
            {
                Name = name,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Category, int>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Category>(category, "تم إضافة القسم بنجاح", 201);
        }

        //* Update Category
        public async Task<ApiResponse<Category>> UpdateCategoryAsync(int categoryId, string name)
        {

            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<Category>("القسم غير موجود", 404);


            var existingCategory = await _unitOfWork.GetRepository<Category, int>().FindAsync(
                c => c.Name == name && c.StoreId == category.StoreId && c.Id != categoryId);
            if (existingCategory != null)
                return new ApiResponse<Category>("اسم القسم موجود بالفعل", 400);


            category.Name = name;
            category.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Category, int>().Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Category>(category, "تم تعديل القسم بنجاح");
        }

        //* Delete Category
        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId)
        {

            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<bool>("القسم غير موجود", 404);


            _unitOfWork.GetRepository<Category, int>().Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف القسم بنجاح");
        }

        //* Restore Category
        public async Task<ApiResponse<bool>> RestoreCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId, onlyDeleted: true);
            if (category == null || !category.IsDeleted)
                return new ApiResponse<bool>("القسم غير موجود أو غير محذوف", 404);


            await _unitOfWork.GetRepository<Category, int>().RestoreAsync(categoryId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع القسم بنجاح");
        }

        //* Get Categories
        public async Task<ApiResponse<List<Category>>> GetCategoriesAsync(int storeId)
        {


            var categories = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.StoreId == storeId);

            if (!categories.Any())
                return new ApiResponse<List<Category>>("لا يوجد أقسام", 404);


            return new ApiResponse<List<Category>>(categories.ToList(), "تم جلب الأقسام بنجاح");
        }

        #endregion

        #region Menu Items

        //* Create Item
        public async Task<ApiResponse<MenuItem>> CreateItemAsync(string name, decimal price, int pointsRequired, int categoryId)
        {
            if (string.IsNullOrEmpty(name) || price <= 0 || pointsRequired < 0 || categoryId <= 0)
                return new ApiResponse<MenuItem>("اسم الصنف، السعر، النقاط المطلوبة يجب أن يكونوا صالحين", 400);


            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<MenuItem>("القسم غير موجود", 404);


            var existingItem = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.Name == name && mi.MenuCategoryId == categoryId);
            if (existingItem != null)
                return new ApiResponse<MenuItem>("الصنف موجود بالفعل", 400);


            var item = new MenuItem
            {
                Name = name,
                Price = price,
                PointsRequired = pointsRequired,
                MenuCategoryId = categoryId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<MenuItem, int>().AddAsync(item);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<MenuItem>(item, "تم إضافة الصنف بنجاح", 201);
        }

        //* Update Item
        public async Task<ApiResponse<MenuItem>> UpdateItemAsync(int itemId, string name, decimal price, int pointsRequired)
        {
            if (itemId <= 0 || string.IsNullOrEmpty(name) || price <= 0 || pointsRequired < 0)
                return new ApiResponse<MenuItem>(" اسم الصنف، السعر، والنقاط المطلوبة يجب أن يكونوا صالحين", 400);


            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId);
            if (item == null)
                return new ApiResponse<MenuItem>("الصنف غير موجود", 404);


            var existingItem = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.Name == name && mi.MenuCategoryId == item.MenuCategoryId && mi.Id != itemId);
            if (existingItem != null)
                return new ApiResponse<MenuItem>("اسم الصنف موجود بالفعل", 400);


            item.Name = name;
            item.Price = price;
            item.PointsRequired = pointsRequired;
            item.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<MenuItem, int>().Update(item);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<MenuItem>(item, "تم تعديل الصنف بنجاح");
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
        public async Task<ApiResponse<List<MenuItem>>> GetItemsAsync(int categoryId)
        {
            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindWithIncludesAsync(
                predicate: mi => mi.MenuCategoryId == categoryId,
                include: q => q.Include(mi => mi.Category));

            if (!items.Any())
                return new ApiResponse<List<MenuItem>>("لا يوجد أصناف", 404);

            return new ApiResponse<List<MenuItem>>(items.ToList(), "تم جلب الأصناف بنجاح");
        }

        //* Get Total Items Count
        public async Task<ApiResponse<int>> GetTotalItemsCountAsync(int storeId)
        {
            var count = (await _unitOfWork.GetRepository<MenuItem, int>().FindWithIncludesAsync(
                mi => mi.Category.StoreId == storeId,
                include: q => q.Include(mi => mi.Category))).Count();

            return new ApiResponse<int>(count, "تم جلب عدد الأصناف بنجاح");
        }

        #endregion
    }
}