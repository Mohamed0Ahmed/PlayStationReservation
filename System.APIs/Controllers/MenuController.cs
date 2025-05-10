using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Shared.DTOs.Menu;

namespace System.APIs.Controllers
{
    [Route("api/menus")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        #region Categories

        //* Get Categories
        [HttpGet("categories/{storeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories(int storeId)
        {
            var response = await _menuService.GetCategoriesAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }
        //* Get deleted Categories
        [HttpGet("categories/deleted/{storeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDeletedCategories(int storeId)
        {
            var response = await _menuService.GetDeletedCategoriesAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Create Category
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto request)
        {
            var response = await _menuService.CreateCategoryAsync(request.Name, request.StoreId);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Category
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto category)
        {
            var response = await _menuService.UpdateCategoryAsync(id, category.Name);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete Category
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _menuService.DeleteCategoryAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Category
        [HttpPut("categories/restore/{id}")]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            var response = await _menuService.RestoreCategoryAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        #endregion

        #region Menu Items

        //* Get Total Items Count
        [AllowAnonymous]
        [HttpGet("items/all/{storeId}")]
        public async Task<IActionResult> GetAllItemsForStore(int storeId)
        {
            var response = await _menuService.GetAllItemsAsync(storeId);
            return StatusCode(response.StatusCode, response);
        }

        //* Create Item
        [HttpPost("items")]
        public async Task<IActionResult> CreateItem([FromBody] ItemDto request)
        {
            var response = await _menuService.CreateItemAsync(request.Name, request.Price, request.PointsRequired, request.CategoryId);
            return StatusCode(response.StatusCode, response);
        }

        //* Update Item
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateMenuItemRequest request)
        {
            var response = await _menuService.UpdateItemAsync(id, request.Name, request.Price, request.PointsRequired);
            return StatusCode(response.StatusCode, response);
        }

        //* Delete Item
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var response = await _menuService.DeleteItemAsync(id);
            return StatusCode(response.StatusCode, response);
        } 

        //* Delete Hard Item
        [HttpDelete("items/hard/{id}")]
        public async Task<IActionResult> DeleteHardItem(int id)
        {
            var response = await _menuService.DeleteHardItemAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Restore Item
        [HttpPut("items/restore/{id}")]
        public async Task<IActionResult> RestoreItem(int id)
        {
            var response = await _menuService.RestoreItemAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        //* Get Items
        [HttpGet("items/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItems(int categoryId)
        {
            var response = await _menuService.GetItemsAsync(categoryId);
            return StatusCode(response.StatusCode, response);
        }  
        
        //* Get Deleted Items
        [HttpGet("items/deleted/{categoryId}")]
        public async Task<IActionResult> GetDeletedItems(int categoryId)
        {
            var response = await _menuService.GetDeletedItemsAsync(categoryId);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}




