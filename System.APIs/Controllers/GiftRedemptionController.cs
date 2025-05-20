using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.GiftRedemptions;

namespace System.APIs.Controllers
{
    [Route("api/redemption")]
    [ApiController]
    public class GiftRedemptionController : ControllerBase
    {
        private readonly IGiftRedemptionService _giftRedemptionService;
        private readonly IGiftService _giftService;

        public GiftRedemptionController(IGiftRedemptionService giftRedemptionService, IGiftService giftService)
        {
            _giftRedemptionService = giftRedemptionService;
            _giftService = giftService;
        }

        // Customer requests a gift redemption
        [HttpPost("request")]
        public async Task<IActionResult> RequestGiftRedemption([FromBody] CreateGiftRedemptionDto dto)
        {
            var response = await _giftRedemptionService.RequestGiftRedemptionAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // Owner updates redemption status (approve/reject)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateRedemptionStatus(int id, [FromBody] UpdateGiftRedemptionStatusDto dto)
        {
            var response = await _giftRedemptionService.UpdateRedemptionStatusAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        // Get pending redemptions for owner
        [HttpGet("store/{storeId}/pending")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetPendingRedemptions(int storeId)
        {
            var response = await _giftRedemptionService.GetPendingRedemptionsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        // Get all redemptions for owner (approved, rejected, and pending)
        [HttpGet("store/{storeId}/all")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetAllRedemptions(int storeId)
        {
            var response = await _giftRedemptionService.GetAllRedemptionsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        // Get customer's redemption history
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerRedemptions(int customerId)
        {
            var response = await _giftRedemptionService.GetCustomerRedemptionsAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        // Get redemption details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRedemptionDetails(int id)
        {
            var response = await _giftRedemptionService.GetRedemptionDetailsAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // Get available gifts for a store
        [HttpGet("store/{storeId}/available-gifts")]
        public async Task<IActionResult> GetAvailableGifts(int storeId)
        {
            var response = await _giftService.GetGiftsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
