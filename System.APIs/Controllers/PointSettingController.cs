using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.PointSettings;


namespace System.APIs.Controllers
{
    [Route("api/settings")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class PointSettingController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public PointSettingController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PointSettingDto setting)
        {
            var response = await _storeService.CreatePointSettingAsync(setting.StoreId, setting.Amount, setting.Points );
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PointSettingDto setting)
        {
            var response = await _storeService.UpdatePointSettingAsync(setting.Id, setting.Amount, setting.Points);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _storeService.DeletePointSettingAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("store/setting/{storeId}")]
        public async Task<IActionResult> Get(int storeId)
        {
            var response = await _storeService.GetPointSettingsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
