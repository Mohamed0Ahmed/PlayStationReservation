using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class AssistanceRequestTypeService : IAssistanceRequestTypeService
    {
        private readonly IRepository<AssistanceRequestType, int> _requestTypeRepository;

        public AssistanceRequestTypeService(IRepository<AssistanceRequestType, int> requestTypeRepository)
        {
            _requestTypeRepository = requestTypeRepository;
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

        public async Task<ApiResponse<List<AssistanceRequestType>>> GetAssistanceRequestTypesAsync(int storeId)
        {
            var requestTypes = await _requestTypeRepository.FindAsync(art => art.StoreId == storeId);
            return new ApiResponse<List<AssistanceRequestType>>(requestTypes.ToList());
        }

        public async Task<ApiResponse<int>> GetTotalAssistanceRequestTypesCountAsync(int storeId)
        {
            var count = (await _requestTypeRepository.FindAsync(art => art.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد أنواع المساعدة بنجاح");
        }
    }
}