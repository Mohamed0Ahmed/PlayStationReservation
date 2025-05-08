using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Customers;


namespace System.APIs.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        #region Customers

        //* Login Customer (allowed without auth)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCustomer([FromBody] CustomerDto customer)
        {
            var response = await _customerService.LoginCustomer(customer.PhoneNumber, customer.StoreId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get customer by phone
        [HttpGet("phone")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> GetCustomerByPhone([FromBody] CustomerDto customer)
        {
            var response = await _customerService.GetCustomerByPhoneAsync(customer.PhoneNumber, customer.StoreId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get customer points
        [HttpGet("{customerId}/points")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> GetCustomerPoints(int customerId)
        {
            var response = await _customerService.GetCustomerPointsAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        //* Update customer
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customer)
        {
            var response = await _customerService.UpdateCustomerAsync(customer.Id, customer.PhoneNumber, customer.StoreId);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete customer
        [HttpDelete("{customerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            var response = await _customerService.DeleteCustomerAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore customer
        [HttpPost("{customerId}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreCustomer(int customerId)
        {
            var response = await _customerService.RestoreCustomerAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}
