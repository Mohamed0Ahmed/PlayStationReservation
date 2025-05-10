using Mapster;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.Assistances;

namespace System.Application.Services
{
    public class AssistanceRequestTypeService : IAssistanceRequestTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssistanceRequestTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Assistance Request Types

        //* Create Assistance Request Type
        public async Task<ApiResponse<AssistanceDto>> CreateAssistanceRequestTypeAsync(string name, int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
                return new ApiResponse<AssistanceDto>("No Store With This Id", 200);

            var existingType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().FindAsync(
                art => art.Name == name && art.StoreId == storeId);

            if (existingType.Any())
                return new ApiResponse<AssistanceDto>("نوع المساعدة موجود بالفعل", 200);


            var requestType = new AssistanceRequestType
            {
                Name = name,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<AssistanceRequestType, int>().AddAsync(requestType);
            await _unitOfWork.SaveChangesAsync();
            var assistanceDto = requestType.Adapt<AssistanceDto>();

            return new ApiResponse<AssistanceDto>(assistanceDto, "تم إضافة نوع المساعدة بنجاح", 201);
        }

        //* Update Assistance Request Type
        public async Task<ApiResponse<AssistanceDto>> UpdateAssistanceRequestTypeAsync(int typeId, string name)
        {
            var requestType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().GetByIdAsync(typeId);
            if (requestType == null)
                return new ApiResponse<AssistanceDto>("نوع المساعدة غير موجود", 204);


            var existingType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().FindAsync(
                art => art.Name == name && art.StoreId == requestType.StoreId && art.Id != typeId);
            if (existingType.Any())
                return new ApiResponse<AssistanceDto>("اسم المساعدة موجود بالفعل", 200);


            requestType.Name = name;
            requestType.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<AssistanceRequestType, int>().Update(requestType);
            await _unitOfWork.SaveChangesAsync();
            var assistanceDto = requestType.Adapt<AssistanceDto>();

            return new ApiResponse<AssistanceDto>(assistanceDto, "تم تعديل نوع المساعدة بنجاح");
        }

        //* Delete Assistance Request Type
        public async Task<ApiResponse<bool>> DeleteAssistanceRequestTypeAsync(int typeId)
        {
            var requestType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().GetByIdAsync(typeId);
            if (requestType == null)
                return new ApiResponse<bool>("نوع المساعدة غير موجود", 204);


            _unitOfWork.GetRepository<AssistanceRequestType, int>().Delete(requestType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف نوع المساعدة بنجاح");
        }

        //* Restore Assistance Request Type
        public async Task<ApiResponse<bool>> RestoreAssistanceRequestTypeAsync(int typeId)
        {
            var requestType = await _unitOfWork.GetRepository<AssistanceRequestType, int>().GetByIdAsync(typeId, onlyDeleted: true);
            if (requestType == null || !requestType.IsDeleted)
                return new ApiResponse<bool>("نوع المساعدة غير موجود أو غير محذوف", 204);


            await _unitOfWork.GetRepository<AssistanceRequestType, int>().RestoreAsync(typeId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع  المساعدة بنجاح");
        }

        //* Get All Assistance Request Types
        public async Task<ApiResponse<IEnumerable<AssistanceDto>>> GetAllAssistanceRequestTypesAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
                return new ApiResponse<IEnumerable<AssistanceDto>>("No Store With This Id", 200);

            var customTypes = await _unitOfWork.GetRepository<AssistanceRequestType, int>()
                .FindAsync(art => art.StoreId == storeId);

            var defaultTypes = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>()
                .GetAllAsync();

            var assistanceDtoIEnumerable = new List<AssistanceDto>();

            // custom types
            assistanceDtoIEnumerable.AddRange(customTypes.Select(ct => new AssistanceDto
            {
                Id = ct.Id,
                Name = ct.Name,
            }));

            // default types
            assistanceDtoIEnumerable.AddRange(defaultTypes.Select(dt => new AssistanceDto
            {
                Id = dt.Id,
                Name = dt.Name,
            }));

            if (assistanceDtoIEnumerable.Count == 0)
                return new ApiResponse<IEnumerable<AssistanceDto>>("لا يوجد أنواع مساعدة", 200);

            return new ApiResponse<IEnumerable<AssistanceDto>>(assistanceDtoIEnumerable, "تم جلب أنواع المساعدة بنجاح");
        }


        //* Get Total Assistance Request Types Count
        public async Task<ApiResponse<int>> GetTotalAssistanceRequestTypesCountAsync(int storeId)
        {
            if (storeId <= 0)
                return new ApiResponse<int>("ادخل البيانات بشكل صحيح", 200);


            var customCount = (await _unitOfWork.GetRepository<AssistanceRequestType, int>().FindAsync(art => art.StoreId == storeId)).Count();
            var defaultCount = (await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetAllAsync()).Count();
            return new ApiResponse<int>(customCount + defaultCount, "تم جلب عدد أنواع المساعدة بنجاح");
        }

        #endregion

        #region Default Assistance Request Types

        //* Create Default Assistance Type
        public async Task<ApiResponse<DefaultAssistanceRequestType>> CreateDefaultAssistanceTypeAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new ApiResponse<DefaultAssistanceRequestType>("اسم المساعدة مطلوب", 200);


            var existingType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().FindAsync(
                dt => dt.Name == name);
            if (existingType.Any())
                return new ApiResponse<DefaultAssistanceRequestType>("نوع المساعدة الثابتة موجود بالفعل", 200);


            var defaultType = new DefaultAssistanceRequestType
            {
                Name = name,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().AddAsync(defaultType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<DefaultAssistanceRequestType>(defaultType, "تم إضافة نوع المساعدة الثابتة بنجاح", 201);
        }

        //* Update Default Assistance Type
        public async Task<ApiResponse<DefaultAssistanceRequestType>> UpdateDefaultAssistanceTypeAsync(int typeId, string name)
        {
            var defaultType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetByIdAsync(typeId);
            if (defaultType == null)
                return new ApiResponse<DefaultAssistanceRequestType>("نوع المساعدة الثابتة غير موجود", 200);


            var existingType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().FindAsync(
                dt => dt.Name == name && dt.Id != typeId);
            if (existingType.Any())
                return new ApiResponse<DefaultAssistanceRequestType>("اسم المساعدة الثابتة موجود بالفعل", 200);


            defaultType.Name = name;
            defaultType.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().Update(defaultType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<DefaultAssistanceRequestType>(defaultType, "تم تعديل نوع المساعدة الثابتة بنجاح");
        }

        //* Delete Default Assistance Type
        public async Task<ApiResponse<bool>> DeleteDefaultAssistanceTypeAsync(int typeId)
        {
            if (typeId <= 0)
                return new ApiResponse<bool>("ادخل البيانات بشكل صحيح", 200);


            var defaultType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetByIdAsync(typeId);
            if (defaultType == null)
                return new ApiResponse<bool>("نوع المساعدة الثابتة غير موجود", 200);


            _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().Delete(defaultType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف نوع المساعدة الثابتة بنجاح");
        }

        //* Restore Default Assistance Type
        public async Task<ApiResponse<bool>> RestoreDefaultAssistanceTypeAsync(int typeId)
        {
            if (typeId <= 0)
                return new ApiResponse<bool>("ادخل البيانات بشكل صحيح", 200);


            var defaultType = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetByIdAsync(typeId, onlyDeleted: true);
            if (defaultType == null || !defaultType.IsDeleted)
                return new ApiResponse<bool>("نوع المساعدة الثابتة غير موجود أو غير محذوف", 200);


            await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().RestoreAsync(typeId);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع نوع المساعدة الثابتة بنجاح");
        }

        //* Get Default Assistance Types
        public async Task<ApiResponse<IEnumerable<AssistanceDto>>> GetDefaultAssistanceTypesAsync()
        {
            var defaultTypes = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetAllAsync();

            if (!defaultTypes.Any())
                return new ApiResponse<IEnumerable<AssistanceDto>>("لا يوجد أنواع مساعدة ثابتة حاليًا", 200);

            var types = defaultTypes.Adapt<IEnumerable<AssistanceDto>>();


            return new ApiResponse<IEnumerable<AssistanceDto>>(types, "تم جلب أنواع المساعدة الثابتة بنجاح");
        }

        //* Get Default deleted Assistance Types
        public async Task<ApiResponse<IEnumerable<AssistanceDto>>> GetDefaultDeletedAssistanceTypesAsync()
        {
            var defaultTypes = await _unitOfWork.GetRepository<DefaultAssistanceRequestType, int>().GetAllAsync(onlyDeleted: true);

            if (!defaultTypes.Any())
                return new ApiResponse<IEnumerable<AssistanceDto>>("لا يوجد أنواع مساعدة محذوفة حاليًا", 200);

            var types = defaultTypes.Adapt<IEnumerable<AssistanceDto>>();

            return new ApiResponse<IEnumerable<AssistanceDto>>(types, "تم جلب أنواع المساعدة المحذوفة بنجاح");
        }

        #endregion
    }
}