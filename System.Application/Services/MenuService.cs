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
                return new ApiResponse<CategoryDto>("No Store With This ID", 200);


            var existingCategory = await _unitOfWork.GetRepository<Category, int>().FindAsync(
                c => c.Name == name && c.StoreId == storeId);
            if (existingCategory.Any())
                return new ApiResponse<CategoryDto>("القسم موجود بالفعل", 200);


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
                return new ApiResponse<CategoryDto>("القسم غير موجود", 200);


            var existingCategory = await _unitOfWork.GetRepository<Category, int>().FindAsync(
                c => c.Name == name && c.StoreId == category.StoreId && c.Id != categoryId);
            if (existingCategory.Any())
                return new ApiResponse<CategoryDto>("اسم القسم موجود بالفعل", 200);


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
                return new ApiResponse<bool>("القسم غير موجود", 200);

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
                return new ApiResponse<bool>("القسم غير موجود أو غير محذوف", 200);

            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(i => i.CategoryId == categoryId ,onlyDeleted:true);


            await _unitOfWork.GetRepository<Category, int>().RestoreAsync(categoryId);
            _unitOfWork.GetRepository<MenuItem, int>().RestoreRange(items);

            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع القسم بنجاح");
        }

        //* Get Categories
        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
                return new ApiResponse<IEnumerable<CategoryDto>>("No Store To Get Categories", 200);


            var categories = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.StoreId == storeId);
            if (!categories.Any())
                return new ApiResponse<IEnumerable<CategoryDto>>("لا يوجد أقسام", 200);

            var categoriesDto = categories.Adapt<IEnumerable<CategoryDto>>();
            return new ApiResponse<IEnumerable<CategoryDto>>(categoriesDto, "تم جلب الأقسام بنجاح");
        }  
        
        //* Get Deleted Categories
        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetDeletedCategoriesAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId );
            if (store == null)
                return new ApiResponse<IEnumerable<CategoryDto>>("No Store To Get Categories", 200);


            var categories = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.StoreId == storeId , onlyDeleted: true);
            if (!categories.Any())
                return new ApiResponse<IEnumerable<CategoryDto>>("لا يوجد أقسام", 200);

            var categoriesDto = categories.Adapt<IEnumerable<CategoryDto>>();
            return new ApiResponse<IEnumerable<CategoryDto>>(categoriesDto, "تم جلب الأقسام بنجاح");
        }

        #endregion

        #region Menu Items

        //* Create Item
        public async Task<ApiResponse<ItemDto>> CreateItemAsync(string name, decimal price, int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<ItemDto>("القسم غير موجود", 200);


            var existingItem = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.Name == name && mi.CategoryId == categoryId);
            if (existingItem.Any())
                return new ApiResponse<ItemDto>("الصنف موجود بالفعل", 200);


            var item = new MenuItem
            {
                Name = name,
                Price = price,
                //PointsRequired = pointsRequired,
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
        public async Task<ApiResponse<ItemDto>> UpdateItemAsync(int itemId, string name, decimal price)
        {

            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId);
            if (item == null)
                return new ApiResponse<ItemDto>("الصنف غير موجود", 200);


            var existingItem = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.Name == name && mi.CategoryId == item.CategoryId && mi.Id != itemId);
            if (existingItem.Any())
                return new ApiResponse<ItemDto>("اسم الصنف موجود بالفعل", 200);


            item.Name = name;
            item.Price = price;
            //item.PointsRequired = pointsRequired;
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
                return new ApiResponse<bool>("الصنف غير موجود", 200);


            _unitOfWork.GetRepository<MenuItem, int>().Delete(item);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف الصنف بنجاح");
        }  
        
        //* Delete Hard Item
        public async Task<ApiResponse<bool>> DeleteHardItemAsync(int itemId)
        {
            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId ,onlyDeleted:true);
            if (item == null)
                return new ApiResponse<bool>("الصنف غير موجود", 200);


            _unitOfWork.GetRepository<MenuItem, int>().DeleteHard(item);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف الصنف بنجاح");
        }

        //* Restore Item
        public async Task<ApiResponse<bool>> RestoreItemAsync(int itemId)
        {
            var item = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(itemId, onlyDeleted: true);
            if (item == null || !item.IsDeleted)
                return new ApiResponse<bool>("الصنف غير موجود أو غير محذوف", 200);


            await _unitOfWork.GetRepository<MenuItem, int>().RestoreAsync(itemId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع الصنف بنجاح");
        }

        //* Get Items
        public async Task<ApiResponse<IEnumerable<ItemDto>>> GetItemsAsync(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.Id == categoryId);
            if (!category.Any())
                return new ApiResponse<IEnumerable<ItemDto>>("هذا القسم غير موجود", 200);


            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(i => i.CategoryId == categoryId);

            if (!items.Any())
                return new ApiResponse<IEnumerable<ItemDto>>("لا يوجد أصناف", 200);

            var itemsDto = items.Adapt<IEnumerable<ItemDto>>();
            return new ApiResponse<IEnumerable<ItemDto>>(itemsDto, "تم جلب الأصناف بنجاح");
        }
        
        //* Get Deleted Items
        public async Task<ApiResponse<IEnumerable<ItemDto>>> GetDeletedItemsAsync(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().FindAsync(c => c.Id == categoryId);
            if (!category.Any())
                return new ApiResponse<IEnumerable<ItemDto>>("هذا القسم غير موجود", 200);


            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(i => i.CategoryId == categoryId , onlyDeleted:true);

            if (!items.Any())
                return new ApiResponse<IEnumerable<ItemDto>>("لا يوجد أصناف", 200);

            var itemsDto = items.Adapt<IEnumerable<ItemDto>>();
            return new ApiResponse<IEnumerable<ItemDto>>(itemsDto, "تم جلب الأصناف بنجاح");
        }

        //* Get All Items Count
        public async Task<ApiResponse<IEnumerable<ItemDto>>> GetAllItemsAsync(int storeId)
        {
            var items = await _unitOfWork.GetRepository<MenuItem, int>().FindAsync(
                mi => mi.StoreId == storeId);
            if (!items.Any())
                return new ApiResponse<IEnumerable<ItemDto>>("لم يتم اضافة اي منتج حاليا",200);


            var itemsDto = items.Adapt<IEnumerable<ItemDto>>();
            return new ApiResponse<IEnumerable<ItemDto>>(itemsDto, "تم جلب كل الأصناف بنجاح");
        }

        #endregion
    }
}