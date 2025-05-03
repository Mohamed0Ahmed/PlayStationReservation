using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs;

namespace System.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAssistanceRequestTypeService _assistanceRequestTypeService;
        private readonly IStoreService _storeService;

        public AdminController(
            IAssistanceRequestTypeService assistanceRequestTypeService,
            IStoreService storeService)
        {
            _assistanceRequestTypeService = assistanceRequestTypeService;
            _storeService = storeService;
        }

        // Default Assistance Types Endpoint *********************************

        #region Default Assistance

        [HttpGet("assistance")]
        public async Task<IActionResult> GetDefaultAssistanceTypes()
        {
            var response = await _assistanceRequestTypeService.GetDefaultAssistanceTypesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("assistance")]
        public async Task<IActionResult> CreateDefaultAssistanceType([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new ApiResponse<DefaultAssistanceRequestType>("اسم المساعدة مطلوب", 400));
            }

            var response = await _assistanceRequestTypeService.CreateDefaultAssistanceTypeAsync(name);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("assistance/{id}")]
        public async Task<IActionResult> UpdateDefaultAssistanceType(int id, [FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new ApiResponse<DefaultAssistanceRequestType>("اسم المساعدة مطلوب", 400));
            }

            var response = await _assistanceRequestTypeService.UpdateDefaultAssistanceTypeAsync(id, name);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("assistance/{id}")]
        public async Task<IActionResult> DeleteDefaultAssistanceType(int id)
        {
            var response = await _assistanceRequestTypeService.DeleteDefaultAssistanceTypeAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        #endregion


        // Store Endpoints *********************************

        #region Stores

        [HttpPost("stores")]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.OwnerEmail))
            {
                return BadRequest(new ApiResponse<Store>("اسم المحل وإيميل الأونر مطلوبين", 400));
            }

            var response = await _storeService.CreateStoreAsync(request.Name, request.OwnerEmail);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("stores/{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.OwnerEmail))
            {
                return BadRequest(new ApiResponse<Store>("اسم المحل وإيميل الأونر مطلوبين", 400));
            }

            var response = await _storeService.UpdateStoreAsync(id, request.Name, request.OwnerEmail);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("stores/{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var response = await _storeService.DeleteStoreAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("stores/restore/{id}")]
        public async Task<IActionResult> RestoreStore(int id)
        {
            var response = await _storeService.RestoreStoreAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("stores")]
        public async Task<IActionResult> GetStores()
        {
            var response = await _storeService.GetStoresAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("stores/deleted")]
        public async Task<IActionResult> GetDeletedStores()
        {
            var response = await _storeService.GetDeletedStoresAsync();
            return StatusCode(response.StatusCode, response);
        }

        #endregion

        // Room Endpoints *********************************

        #region Rooms

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (request.StoreId <= 0 || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ApiResponse<Room>("اضف يوزر نيم وباسورد للغرفة ", 400));
            }

            var response = await _storeService.CreateRoomAsync(request.StoreId, request.Username, request.Password);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ApiResponse<Room>("اسم المستخدم وكلمة المرور مطلوبين", 400));
            }

            var response = await _storeService.UpdateRoomAsync(id, request.Username, request.Password);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var response = await _storeService.DeleteRoomAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut("rooms/restore/{id}")]
        public async Task<IActionResult> RestoreRoom(int id)
        {
            var response = await _storeService.RestoreRoomAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("rooms/{storeId}")]
        public async Task<IActionResult> GetRooms(int storeId)
        {
            if (storeId <= 0)
            {
                return BadRequest(new ApiResponse<List<Room>>("معرف المحل غير صالح", 400));
            }

            var response = await _storeService.GetRoomsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
  
}