using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared;
using System.Shared.DTOs.Orders;

namespace System.APIs.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize] 
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #region Orders

        //* Create Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var roomIdClaim = User.FindFirst("roomId")?.Value;
            var storeIdClaim = User.FindFirst("storeId")?.Value;

            if (string.IsNullOrEmpty(roomIdClaim) || string.IsNullOrEmpty(storeIdClaim))
                return Unauthorized(new ApiResponse<object>("التوكن غير صالح أو لا يحتوي على بيانات الغرفة", 200));

            if (!int.TryParse(roomIdClaim, out int roomId) || !int.TryParse(storeIdClaim, out int storeId))
                return BadRequest(new ApiResponse<object>("بيانات الغرفة أو المتجر غير صالحة", 200));

            var response = await _orderService.CreateOrderAsync(request.PhoneNumber, roomId, request.Items);


            return StatusCode(response.StatusCode, response);
        }

        //* Get Pending Orders
        [HttpGet("pending/{storeId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetPendingOrders(int storeId)
        {
            var response = await _orderService.GetPendingOrdersAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }


        //* Get All Orders
        [HttpGet("store/{storeId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetOrders(int storeId)
        {
            var response = await _orderService.GetOrdersAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }


        //* Approve Order
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var response = await _orderService.ApproveOrderAsync(id);
            return StatusCode(response.StatusCode, response);
        }


        //* Reject Order
        [HttpPut("reject/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> RejectOrder(int id, [FromBody] RejectDto rejectDto)
        {
            var response = await _orderService.RejectOrderAsync(id, rejectDto.Reason);
            return StatusCode(response.StatusCode, response);
        }


        //* Get Total Orders Count
        [HttpGet("count/{storeId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetTotalOrdersCount(int storeId)
        {
            var response = await _orderService.GetTotalOrdersCountAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}