using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs;

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
        public async Task<IActionResult> CreateGift([FromBody] CreateGiftRequest request)
        {
            var response = await _giftService.CreateGiftAsync(request.Name, request.PointsRequired, request.StoreId);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Gift
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGift(int id, [FromBody] UpdateGiftRequest request)
        {
            var response = await _giftService.UpdateGiftAsync(id, request.Name, request.PointsRequired);
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

        //* Get Most Requested Gift Count
        [HttpGet("most-requested/{storeId}")]
        public async Task<IActionResult> GetMostRequestedGiftCount(int storeId)
        {
            var response = await _giftService.GetMostRequestedGiftCountAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}



