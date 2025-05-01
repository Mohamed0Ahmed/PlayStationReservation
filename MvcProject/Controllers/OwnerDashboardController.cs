using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Domain.Enums;
using MvcProject.Models;

namespace MvcProject.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerDashboardController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IStoreService _storeService;
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly IOrderService _orderService;
        private readonly IAssistanceRequestService _assistanceRequestService;
        private readonly ILogger<OwnerDashboardController> _logger;

        public OwnerDashboardController(
            UserManager<IdentityUser> userManager,
            IStoreService storeService,
            IMenuCategoryService menuCategoryService,
            IMenuItemService menuItemService,
            IOrderService orderService,
            IAssistanceRequestService assistanceRequestService,
            ILogger<OwnerDashboardController> logger)
        {
            _userManager = userManager;
            _storeService = storeService;
            _menuCategoryService = menuCategoryService;
            _menuItemService = menuItemService;
            _orderService = orderService;
            _assistanceRequestService = assistanceRequestService;
            _logger = logger;
        }

        private void CheckForErrorMessage()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"]?.ToString();
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var store = await _storeService.GetStoreByOwnerEmailAsync(user.Email);
                if (store == null)
                {
                    TempData["ErrorMessage"] = "Store not found for this owner.";
                    return RedirectToAction("Index", "Home");
                }

                var categories = await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id);
                var orders = await _orderService.GetOrdersByStoreAsync(store.Id);
                var assistanceRequests = await _assistanceRequestService.GetAssistanceRequestsByStoreAsync(store.Id);

                var viewModel = new OwnerDashboardViewModel
                {
                    StoreId = store.Id,
                    StoreName = store.Name,
                    Categories = categories.ToList(),
                    Orders = orders.ToList(),
                    AssistanceRequests = assistanceRequests.ToList()
                };

                CheckForErrorMessage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the owner dashboard.");
                TempData["ErrorMessage"] = "An unexpected error occurred while loading the dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                var user = await _userManager.GetUserAsync(User);
                var store = await _storeService.GetStoreByOwnerEmailAsync(user.Email);

                // Ensure the order belongs to the owner's store
                if (order.Customer.StoreId != store.Id)
                {
                    return NotFound();
                }

                var orderDetails = new
                {
                    id = order.Id,
                    customerPhone = order.Customer.PhoneNumber,
                    roomUsername = order.Room.Username,
                    totalAmount = order.TotalAmount,
                    paymentMethod = order.PaymentMethod,
                    pointsUsed = order.PointsUsed,
                    status = order.Status.ToString()
                };

                return Json(orderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order details for order {OrderId}", id);
                return StatusCode(500, "Error fetching order details.");
            }
        }

        public IActionResult CreateCategory(int storeId)
        {
            return View(new MenuCategoryViewModel { StoreId = storeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(MenuCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var category = new MenuCategory
                {
                    Name = model.Name,
                    StoreId = model.StoreId
                };
                await _menuCategoryService.AddMenuCategoryAsync(category);
                TempData["SuccessMessage"] = "Menu category created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a menu category.");
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return View(model);
            }
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            try
            {
                var category = await _menuCategoryService.GetMenuCategoryByIdAsync(id);
                var model = new MenuCategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    StoreId = category.StoreId
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving menu category {CategoryId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, MenuCategoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var category = new MenuCategory
                {
                    Id = model.Id,
                    Name = model.Name,
                    StoreId = model.StoreId
                };
                await _menuCategoryService.UpdateMenuCategoryAsync(category);
                TempData["SuccessMessage"] = "Menu category updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating menu category {CategoryId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _menuCategoryService.DeleteMenuCategoryAsync(id);
                TempData["SuccessMessage"] = "Menu category deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting menu category {CategoryId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult CreateMenuItem(int categoryId)
        {
            return View(new MenuItemViewModel { MenuCategoryId = categoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuItem(MenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var menuItem = new MenuItem
                {
                    Name = model.Name,
                    Price = model.Price,
                    PointsRequired = model.PointsRequired,
                    MenuCategoryId = model.MenuCategoryId
                };
                await _menuItemService.AddMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Menu item created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a menu item.");
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return View(model);
            }
        }

        public async Task<IActionResult> EditMenuItem(int id)
        {
            try
            {
                var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
                var model = new MenuItemViewModel
                {
                    Id = menuItem.Id,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    PointsRequired = menuItem.PointsRequired,
                    MenuCategoryId = menuItem.MenuCategoryId
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving menu item {MenuItemId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItem(int id, MenuItemViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var menuItem = new MenuItem
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    PointsRequired = model.PointsRequired,
                    MenuCategoryId = model.MenuCategoryId
                };
                await _menuItemService.UpdateMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Menu item updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating menu item {MenuItemId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                await _menuItemService.DeleteMenuItemAsync(id);
                TempData["SuccessMessage"] = "Menu item deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting menu item {MenuItemId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(OrderStatusUpdateViewModel model)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(model.Id);
                if (!Enum.TryParse<OrderStatus>(model.Status, out var status))
                {
                    TempData["ErrorMessage"] = "Invalid order status.";
                    return RedirectToAction("Index");
                }

                order.Status = status;
                order.RejectionReason = model.RejectionReason;
                order.LastModifiedOn = DateTime.UtcNow;

                await _orderService.UpdateOrderAsync(order);
                TempData["SuccessMessage"] = "Order status updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order status for order {OrderId}.", model.Id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssistanceRequestStatus(AssistanceRequestStatusUpdateViewModel model)
        {
            try
            {
                var assistanceRequest = await _assistanceRequestService.GetAssistanceRequestByIdAsync(model.Id);
                if (!Enum.TryParse<AssistanceRequestStatus>(model.Status, out var status))
                {
                    TempData["ErrorMessage"] = "Invalid assistance request status.";
                    return RedirectToAction("Index");
                }

                assistanceRequest.Status = status;
                assistanceRequest.RejectionReason = model.RejectionReason;
                assistanceRequest.LastModifiedOn = DateTime.UtcNow;

                await _assistanceRequestService.UpdateAssistanceRequestAsync(assistanceRequest);
                TempData["SuccessMessage"] = "Assistance request status updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating assistance request status for request {RequestId}.", model.Id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}