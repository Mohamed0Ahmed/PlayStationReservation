using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class AssistanceRequestTypeService : IAssistanceRequestTypeService
    {
        private readonly IRepository<AssistanceRequestType, int> _requestTypeRepository;
        private readonly IRepository<DefaultAssistanceRequestType, int> _defaultRequestTypeRepository;

        public AssistanceRequestTypeService(
            IRepository<AssistanceRequestType, int> requestTypeRepository,
            IRepository<DefaultAssistanceRequestType, int> defaultRequestTypeRepository)
        {
            _requestTypeRepository = requestTypeRepository;
            _defaultRequestTypeRepository = defaultRequestTypeRepository;
        }

        public async Task<ApiResponse<AssistanceRequestType>> CreateAssistanceRequestTypeAsync(string name, int storeId)
        {
            var requestType = new AssistanceRequestType
            {
                Name = name,
                StoreId = storeId,
                CreatedOn = DateTime.UtcNow
            };

            await _requestTypeRepository.AddAsync(requestType);
            return new ApiResponse<AssistanceRequestType>(requestType, "تم إضافة نوع المساعدة بنجاح", 201);
        }

        public async Task<ApiResponse<AssistanceRequestType>> UpdateAssistanceRequestTypeAsync(int typeId, string name)
        {
            var requestType = await _requestTypeRepository.GetByIdAsync(typeId);
            if (requestType == null)
            {
                return new ApiResponse<AssistanceRequestType>("نوع المساعدة غير موجود", 404);
            }

            requestType.Name = name;
            requestType.LastModifiedOn = DateTime.UtcNow;
            _requestTypeRepository.Update(requestType);

            return new ApiResponse<AssistanceRequestType>(requestType, "تم تعديل نوع المساعدة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteAssistanceRequestTypeAsync(int typeId)
        {
            var requestType = await _requestTypeRepository.GetByIdAsync(typeId);
            if (requestType == null)
            {
                return new ApiResponse<bool>("نوع المساعدة غير موجود", 404);
            }

            _requestTypeRepository.Delete(requestType);
            return new ApiResponse<bool>(true, "تم حذف نوع المساعدة بنجاح");
        }

        public async Task<ApiResponse<List<AssistanceRequestType>>> GetAllAssistanceRequestTypesAsync(int storeId)
        {
            var customTypes = await _requestTypeRepository.FindAsync(art => art.StoreId == storeId);

            var defaultTypes = await _defaultRequestTypeRepository.GetAllAsync();
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
            var customCount = (await _requestTypeRepository.FindAsync(art => art.StoreId == storeId)).Count();
            var defaultCount = (await _defaultRequestTypeRepository.GetAllAsync()).Count();
            return new ApiResponse<int>(customCount + defaultCount, "تم جلب عدد أنواع المساعدة بنجاح");
        }
    }
}