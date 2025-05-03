using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs;

namespace System.APIs.Controllers
{
    [Route("api/stores")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        #region Stores

        //* Create Store
        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request)
        {
            var response = await _storeService.CreateStoreAsync(request.Name, request.OwnerEmail);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Store
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreRequest request)
        {
            var response = await _storeService.UpdateStoreAsync(id, request.Name, request.OwnerEmail);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete Store
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var response = await _storeService.DeleteStoreAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Store
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreStore(int id)
        {
            var response = await _storeService.RestoreStoreAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Stores
        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var response = await _storeService.GetStoresAsync();
            return StatusCode(response.StatusCode, response);
        }

        //* Get Deleted Stores
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedStores()
        {
            var response = await _storeService.GetDeletedStoresAsync();
            return StatusCode(response.StatusCode, response);
        }

        #endregion

        #region Rooms

        //* Create Room
        [HttpPost("{storeId}/rooms")]
        public async Task<IActionResult> CreateRoom(int storeId, [FromBody] CreateRoomRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ApiResponse<Room>("اسم المستخدم وكلمة المرور مطلوبين", 400));
            }

            var response = await _storeService.CreateRoomAsync(storeId, request.Username, request.Password);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Room
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

        //* Delete Room
        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var response = await _storeService.DeleteRoomAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Room
        [HttpPut("rooms/restore/{id}")]
        public async Task<IActionResult> RestoreRoom(int id)
        {
            var response = await _storeService.RestoreRoomAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Rooms
        [Authorize(Roles = "Owner")]
        [HttpGet("{storeId}/rooms")]
        public async Task<IActionResult> GetRooms(int storeId)
        {
            var response = await _storeService.GetRoomsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}