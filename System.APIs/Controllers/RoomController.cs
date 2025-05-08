using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Rooms;


namespace System.APIs.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoomController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public RoomController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            var response = await _storeService.CreateRoomAsync(request.StoreId, request.Username, request.Password);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
        {
            var response = await _storeService.UpdateRoomAsync(id, request.Username, request.Password);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var response = await _storeService.DeleteRoomAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreRoom(int id)
        {
            var response = await _storeService.RestoreRoomAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("store/{storeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRooms(int storeId)
        {
            var response = await _storeService.GetRoomsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("store/deleted/{storeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDeletedRooms(int storeId)
        {
            var response = await _storeService.GetDeletedRoomsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
