using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Assistances;

namespace System.APIs.Controllers
{
    [Route("api/assistance-requests")]
    [ApiController]
    public class AssistanceRequestController : ControllerBase
    {
        private readonly IAssistanceRequestService _assistanceRequestService;

        public AssistanceRequestController(IAssistanceRequestService assistanceRequestService)
        {
            _assistanceRequestService = assistanceRequestService;
        }

        #region Assistance Requests

        //* Create Assistance Request (For Customer)
        [HttpPost]
        [AllowAnonymous] // Allow customers to create assistance requests
        public async Task<IActionResult> CreateAssistanceRequest([FromBody] CreateAssistanceRequest request)
        {
            var response = await _assistanceRequestService.CreateAssistanceRequestAsync(request.RoomId, request.RequestTypeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Pending Assistance Requests
        [HttpGet("pending/{storeId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetPendingAssistanceRequests(int storeId)
        {
            var response = await _assistanceRequestService.GetPendingAssistanceRequestsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Get All Assistance Requests
        [HttpGet("store/{storeId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetAssistanceRequests(int storeId, [FromQuery] bool includeDeleted = false)
        {
            var response = await _assistanceRequestService.GetAssistanceRequestsAsync(storeId, includeDeleted);
            return StatusCode(response.StatusCode, response);
        }

        //* Approve Assistance Request
        [HttpPut("approve/{id}")]
        [Authorize(Roles =  "Owner")]
        public async Task<IActionResult> ApproveAssistanceRequest(int id)
        {
            var response = await _assistanceRequestService.ApproveAssistanceRequestAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Reject Assistance Request
        [HttpPut("reject/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> RejectAssistanceRequest(int id, [FromBody] string rejectionReason)
        {
            var response = await _assistanceRequestService.RejectAssistanceRequestAsync(id, rejectionReason);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Total Assistance Requests Count
        [HttpGet("count/{storeId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetTotalAssistanceRequestsCount(int storeId)
        {
            var response = await _assistanceRequestService.GetTotalAssistanceRequestsCountAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}

