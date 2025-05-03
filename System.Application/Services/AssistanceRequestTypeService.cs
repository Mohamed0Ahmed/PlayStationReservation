using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Infrastructure.Unit;
using System.Shared;

namespace System.Application.Services
{
    public class AssistanceRequestTypeService : IAssistanceRequestTypeService
    {
        private readonly IRepository<AssistanceRequestType, int> _assistanceRequestType;
        private readonly IRepository<DefaultAssistanceRequestType, int> _defaultAssistanceRequestType;
        private readonly IUnitOfWork _unitOfWork;

        public AssistanceRequestTypeService(
            IRepository<AssistanceRequestType, int> assistanceRequestTypeRepository,
            IRepository<DefaultAssistanceRequestType, int> defaultAssistanceRequestTypeRepository,
            IUnitOfWork unitOfWork)
        {
            _assistanceRequestType = assistanceRequestTypeRepository;
            _defaultAssistanceRequestType = defaultAssistanceRequestTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AssistanceRequestType>> CreateAssistanceRequestTypeAsync(string name, int storeId)
        {
            var requestType = new AssistanceRequestType
            {
                Name = name,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _assistanceRequestType.AddAsync(requestType);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<AssistanceRequestType>(requestType, "تم إضافة المساعدة بنجاح", 201);
        }

        public async Task<ApiResponse<AssistanceRequestType>> UpdateAssistanceRequestTypeAsync(int typeId, string name)
        {
            var requestType = await _assistanceRequestType.GetByIdAsync(typeId);
            if (requestType == null)
            {
                return new ApiResponse<AssistanceRequestType>(" المساعدة غير موجودة", 404);
            }

            requestType.Name = name;
            requestType.LastModifiedOn = DateTime.UtcNow;
            _assistanceRequestType.Update(requestType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<AssistanceRequestType>(requestType, "تم تعديل المساعدة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteAssistanceRequestTypeAsync(int typeId)
        {
            var requestType = await _assistanceRequestType.GetByIdAsync(typeId);
            if (requestType == null)
            {
                return new ApiResponse<bool>(" المساعدة غير موجودة", 404);
            }

            _assistanceRequestType.Delete(requestType);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<bool>(true, "تم حذف  المساعدة بنجاح");
        }

        public async Task<ApiResponse<List<AssistanceRequestType>>> GetAllAssistanceRequestTypesAsync(int storeId)
        {
            var customTypes = await _assistanceRequestType.FindAsync(art => art.StoreId == storeId);
            var defaultTypes = await _defaultAssistanceRequestType.GetAllAsync();
            var allTypes = customTypes.Concat(defaultTypes.Select(dt => new AssistanceRequestType
            {
                Id = 0, // Default types won't have real IDs in this context
                Name = dt.Name,
                StoreId = storeId,
                CreatedOn = dt.CreatedOn
            })).ToList();

            return new ApiResponse<List<AssistanceRequestType>>(allTypes);
        }

        public async Task<ApiResponse<int>> GetTotalAssistanceRequestTypesCountAsync(int storeId)
        {
            var customCount = (await _assistanceRequestType.FindAsync(art => art.StoreId == storeId)).Count();
            var defaultCount = (await _defaultAssistanceRequestType.GetAllAsync()).Count();
            return new ApiResponse<int>(customCount + defaultCount, "تم جلب عدد أنواع المساعدة بنجاح");
        }

        // New methods for Default Assistance Types
        public async Task<ApiResponse<DefaultAssistanceRequestType>> CreateDefaultAssistanceTypeAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ApiResponse<DefaultAssistanceRequestType>("اسم المساعدة مطلوب", 400);
            }

            var defaultType = new DefaultAssistanceRequestType
            {
                Name = name,
                CreatedOn = DateTime.UtcNow
            };

            await _defaultAssistanceRequestType.AddAsync(defaultType);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<DefaultAssistanceRequestType>(defaultType, "تم إضافة نوع المساعدة الثابتة بنجاح", 201);
        }

        public async Task<ApiResponse<DefaultAssistanceRequestType>> UpdateDefaultAssistanceTypeAsync(int typeId, string name)
        {
            var defaultType = await _defaultAssistanceRequestType.GetByIdAsync(typeId);
            if (defaultType == null)
            {
                return new ApiResponse<DefaultAssistanceRequestType>("نوع المساعدة الثابتة غير موجود", 404);
            }

            defaultType.Name = name;
            defaultType.LastModifiedOn = DateTime.UtcNow;
            _defaultAssistanceRequestType.Update(defaultType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<DefaultAssistanceRequestType>(defaultType, "تم تعديل نوع المساعدة الثابتة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteDefaultAssistanceTypeAsync(int typeId)
        {
            var defaultType = await _defaultAssistanceRequestType.GetByIdAsync(typeId);
            if (defaultType == null)
            {
                return new ApiResponse<bool>("نوع المساعدة الثابتة غير موجود", 404);
            }

            _defaultAssistanceRequestType.Delete(defaultType);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<bool>(true, "تم حذف نوع المساعدة الثابتة بنجاح");
        }

        public async Task<ApiResponse<List<DefaultAssistanceRequestType>>> GetDefaultAssistanceTypesAsync()
        {
            var defaultTypes = await _defaultAssistanceRequestType.GetAllAsync();

            if (!defaultTypes.Any())
                return new ApiResponse<List<DefaultAssistanceRequestType>>("لا يوجد مساعدات ثابتة حاليا", 404);

            return new ApiResponse<List<DefaultAssistanceRequestType>>(defaultTypes.ToList(), "تم جلب أنواع المساعدة الثابتة بنجاح");
        }
    }
}