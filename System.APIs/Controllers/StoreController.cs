using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Stores;

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

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] StoreDto request)
        {
            var response = await _storeService.CreateStoreAsync(request.Name, request.OwnerEmail);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] StoreDto request)
        {
            var response = await _storeService.UpdateStoreAsync(id, request.Name, request.OwnerEmail);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var response = await _storeService.DeleteStoreAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreStore(int id)
        {
            var response = await _storeService.RestoreStoreAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var response = await _storeService.GetStoresAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedStores()
        {
            var response = await _storeService.GetDeletedStoresAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
}
