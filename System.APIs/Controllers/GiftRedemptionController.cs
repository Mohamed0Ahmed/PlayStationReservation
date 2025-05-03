using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs;

namespace System.APIs.Controllers
{
    [Route("api/gift-redeem")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class GiftRedemptionController : ControllerBase
    {
        private readonly IGiftRedemptionService _giftRedemptionService;

        public GiftRedemptionController(IGiftRedemptionService giftRedemptionService)
        {
            _giftRedemptionService = giftRedemptionService;
        }

        #region Gift Redemptions

        //* Create Gift Redemption (For Customer)
        [HttpPost]
        [AllowAnonymous] 
        public async Task<IActionResult> CreateGiftRedemption([FromBody] CreateGiftRedemptionRequest request)
        {
            var response = await _giftRedemptionService.CreateGiftRedemptionAsync(request.PhoneNumber, request.GiftId, request.RoomId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Pending Gift Redemptions
        [HttpGet("pending/{storeId}")]
        public async Task<IActionResult> GetPendingGiftRedemptions(int storeId)
        {
            var response = await _giftRedemptionService.GetPendingGiftRedemptionsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get All Gift Redemptions
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetGiftRedemptions(int storeId, [FromQuery] bool includeDeleted = false)
        {
            var response = await _giftRedemptionService.GetGiftRedemptionsAsync(storeId, includeDeleted);
            return StatusCode(response.StatusCode, response);
        }

        //* Approve Gift Redemption
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveGiftRedemption(int id)
        {
            var response = await _giftRedemptionService.ApproveGiftRedemptionAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Reject Gift Redemption
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectGiftRedemption(int id, [FromBody] string rejectionReason)
        {
            var response = await _giftRedemptionService.RejectGiftRedemptionAsync(id, rejectionReason);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Total Gift Redemptions Count
        [HttpGet("count/{storeId}")]
        public async Task<IActionResult> GetTotalGiftRedemptionsCount(int storeId)
        {
            var response = await _giftRedemptionService.GetTotalGiftRedemptionsCountAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Total Points Redeemed
        [HttpGet("points-redeemed/{storeId}")]
        public async Task<IActionResult> GetTotalPointsRedeemed(int storeId)
        {
            var response = await _giftRedemptionService.GetTotalPointsRedeemedAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}

