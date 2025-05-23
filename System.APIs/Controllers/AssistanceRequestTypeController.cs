﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Assistances;

namespace System.APIs.Controllers
{
    [Route("api/assistance")]
    [ApiController]
    public class AssistanceRequestTypeController : ControllerBase
    {
        private readonly IAssistanceRequestTypeService _assistanceRequestTypeService;

        public AssistanceRequestTypeController(IAssistanceRequestTypeService assistanceRequestTypeService)
        {
            _assistanceRequestTypeService = assistanceRequestTypeService;
        }

        #region Default Assistance Request Types

        //* Get Default Assistance Types
        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAssistanceTypes()
        {
            var response = await _assistanceRequestTypeService.GetDefaultAssistanceTypesAsync();
            return StatusCode(response.StatusCode, response);
        }
        
        //* Get Default Deleted Assistance Types
        [Authorize(Roles = "Admin")]
        [HttpGet("default/deleted")]
        public async Task<IActionResult> GetDefaultDeletedAssistanceTypes()
        {
            var response = await _assistanceRequestTypeService.GetDefaultDeletedAssistanceTypesAsync();
            return StatusCode(response.StatusCode, response);
        }

        //* Create Default Assistance Type
        [Authorize(Roles = "Admin")]
        [HttpPost("default")]
        public async Task<IActionResult> CreateDefaultAssistanceType([FromBody] AssistanceDto assistance)
        {
            var response = await _assistanceRequestTypeService.CreateDefaultAssistanceTypeAsync(assistance.Name);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Default Assistance Type
        [Authorize(Roles = "Admin")]
        [HttpPut("default/{id}")]
        public async Task<IActionResult> UpdateDefaultAssistanceType(int id, [FromBody] AssistanceDto assistance)
        {
            var response = await _assistanceRequestTypeService.UpdateDefaultAssistanceTypeAsync(id, assistance.Name);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete Default Assistance Type
        [Authorize(Roles = "Admin")]
        [HttpDelete("default/{id}")]
        public async Task<IActionResult> DeleteDefaultAssistanceType(int id)
        {
            var response = await _assistanceRequestTypeService.DeleteDefaultAssistanceTypeAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Default Assistance Type
        [Authorize(Roles = "Admin")]
        [HttpPut("default/restore/{id}")]
        public async Task<IActionResult> RestoreDefaultAssistanceType(int id)
        {
            var response = await _assistanceRequestTypeService.RestoreDefaultAssistanceTypeAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        #endregion

        #region Custom Assistance Request Types

        //* Get All Assistance Request Types
        //[Authorize(Roles = "Owner")]
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetAllAssistanceRequestTypes(int storeId)
        {
            var response = await _assistanceRequestTypeService.GetAllAssistanceRequestTypesAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }  
        
        //* Get All Deleted Assistance Request Types
        [Authorize(Roles = "Owner")]
        [HttpGet("store/deleted/{storeId}")]
        public async Task<IActionResult> GetAllDeletedAssistanceRequestTypes(int storeId)
        {
            var response = await _assistanceRequestTypeService.GetAllDeletedAssistanceRequestTypesAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Create Assistance Request Type
        [Authorize(Roles = "Owner")]
        [HttpPost("store/{storeId}")]
        public async Task<IActionResult> CreateAssistanceRequestType(int storeId, [FromBody] AssistanceDto assistance)
        {
            var response = await _assistanceRequestTypeService.CreateAssistanceRequestTypeAsync(assistance.Name, storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Assistance Request Type
        [Authorize(Roles = "Owner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssistanceRequestType(int id, [FromBody] AssistanceDto assistance)
        {
            var response = await _assistanceRequestTypeService.UpdateAssistanceRequestTypeAsync(id, assistance.Name);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete Assistance Request Type
        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssistanceRequestType(int id)
        {
            var response = await _assistanceRequestTypeService.DeleteAssistanceRequestTypeAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Assistance Request Type
        [Authorize(Roles = "Owner")]
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreAssistanceRequestType(int id)
        {
            var response = await _assistanceRequestTypeService.RestoreAssistanceRequestTypeAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Total Assistance Request Types Count
        [Authorize(Roles = "Owner")]
        [HttpGet("count/{storeId}")]
        public async Task<IActionResult> GetTotalAssistanceRequestTypesCount(int storeId)
        {
            var response = await _assistanceRequestTypeService.GetTotalAssistanceRequestTypesCountAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}