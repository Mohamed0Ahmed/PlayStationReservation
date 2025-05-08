using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Gifts;

namespace System.APIs.Controllers
{
    [Route("api/gifts")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        #region Gifts

        //* Create Gift
        [HttpPost]
        public async Task<IActionResult> CreateGift([FromBody] GiftDto request)
        {
            var response = await _giftService.CreateGiftAsync(request.Name, request.PointsRequired, request.StoreId);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Gift
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGift([FromBody] GiftDto request)
        {
            var response = await _giftService.UpdateGiftAsync(request.Id, request.Name, request.PointsRequired);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete Gift
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var response = await _giftService.DeleteGiftAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Gift
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreGift(int id)
        {
            var response = await _giftService.RestoreGiftAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Gifts
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetGifts(int storeId)
        {
            var response = await _giftService.GetGiftsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Total Gifts Count
        [HttpGet("count/{storeId}")]
        public async Task<IActionResult> GetTotalGiftsCount(int storeId)
        {
            var response = await _giftService.GetTotalGiftsCountAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}



