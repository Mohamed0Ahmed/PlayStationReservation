using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Orders;

namespace System.APIs.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #region Orders

        [HttpPost]
        [AllowAnonymous] // Allow customers to create orders
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || request.RoomId <= 0 || request.Items == null || !request.Items.Any())
            {
                return BadRequest(new ApiResponse<Order>("رقم التليفون، معرف الغرفة، والأصناف يجب أن تكون صالحة", 400));
            }

            var response = await _orderService.CreateOrderAsync(request.PhoneNumber, request.RoomId, request.Items);
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
        public async Task<IActionResult> GetOrders(int storeId, [FromQuery] bool includeDeleted = false)
        {
            var response = await _orderService.GetOrdersAsync(storeId, includeDeleted);
            return StatusCode(response.StatusCode, response);
        }

        //* Approve Order
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ApproveOrder(int id, [FromBody] decimal totalAmount)
        {
            if (totalAmount < 0)
            {
                return BadRequest(new ApiResponse<Order>("المبلغ الإجمالي يجب أن يكون صالحًا", 400));
            }

            var response = await _orderService.ApproveOrderAsync(id, totalAmount);
            return StatusCode(response.StatusCode, response);
        }

        //* Reject Order
        [HttpPut("reject/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> RejectOrder(int id, [FromBody] string rejectionReason)
        {
            if (string.IsNullOrEmpty(rejectionReason))
            {
                return BadRequest(new ApiResponse<Order>("سبب الرفض مطلوب", 400));
            }

            var response = await _orderService.RejectOrderAsync(id, rejectionReason);
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

