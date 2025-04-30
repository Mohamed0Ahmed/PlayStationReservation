using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using MvcProject.Models;
using System.Domain.Enums;

namespace MvcProject.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerDashboardController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly IOrderService _orderService;
        private readonly IAssistanceRequestService _assistanceRequestService;
        private readonly IRoomService _roomService;
        private readonly UserManager<IdentityUser> _userManager;

        public OwnerDashboardController(
            IStoreService storeService,
            IMenuCategoryService menuCategoryService,
            IMenuItemService menuItemService,
            IOrderService orderService,
            IAssistanceRequestService assistanceRequestService,
            IRoomService roomService,
            UserManager<IdentityUser> userManager)
        {
            _storeService = storeService;
            _menuCategoryService = menuCategoryService;
            _menuItemService = menuItemService;
            _orderService = orderService;
            _assistanceRequestService = assistanceRequestService;
            _roomService = roomService;
            _userManager = userManager;
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
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Find the store associated with the owner
                var store = (await _storeService.GetStoreByIdAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null)
                {
                    TempData["ErrorMessage"] = "No store found for this owner.";
                    return View(new OwnerDashboardViewModel());
                }

                // Get menu categories with items
                var categories = await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id);

                // Get all rooms for the store
                var rooms = (await _roomService.FindRoomAsync(r => r.StoreId == store.Id && !r.IsDeleted)).ToList();

                // Get orders for all rooms in the store
                var orders = new List<Order>();
                foreach (var room in rooms)
                {
                    var roomOrders = await _orderService.GetOrdersByRoomAsync(room.Id);
                    orders.AddRange(roomOrders);
                }

                // Get assistance requests
                var assistanceRequests = await _assistanceRequestService.FindAssistanceRequestAsync(a => a.Room.StoreId == store.Id && !a.IsDeleted);

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
                TempData["ErrorMessage"] = "An unexpected error occurred while loading the dashboard. Please try again later.";
                CheckForErrorMessage();
                return View(new OwnerDashboardViewModel());
            }
        }

        // Add Category
        public IActionResult AddCategory()
        {
            return View(new MenuCategoryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(MenuCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null)
                {
                    TempData["ErrorMessage"] = "No store found for this owner.";
                    return RedirectToAction("Index");
                }

                var category = new MenuCategory
                {
                    Name = model.Name,
                    StoreId = store.Id
                };

                await _menuCategoryService.AddMenuCategoryAsync(category);
                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                CheckForErrorMessage();
                return View(model);
            }
        }

        // Edit Category
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _menuCategoryService.GetMenuCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction("Index");
            }

            var model = new MenuCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(MenuCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var category = await _menuCategoryService.GetMenuCategoryByIdAsync(model.Id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                category.Name = model.Name;
                await _menuCategoryService.UpdateMenuCategoryAsync(category);
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                CheckForErrorMessage();
                return View(model);
            }
        }

        // Delete Category
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _menuCategoryService.DeleteMenuCategoryAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the category.";
            }
            return RedirectToAction("Index");
        }

        // Add Menu Item
        public async Task<IActionResult> AddMenuItem()
        {
            var user = await _userManager.GetUserAsync(User);
            var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
            if (store == null)
            {
                TempData["ErrorMessage"] = "No store found for this owner.";
                return RedirectToAction("Index");
            }

            var categories = await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id);
            var model = new MenuItemViewModel
            {
                Categories = categories.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenuItem(MenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                model.Categories = (await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id)).ToList();
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null)
                {
                    TempData["ErrorMessage"] = "No store found for this owner.";
                    return RedirectToAction("Index");
                }

                var category = await _menuCategoryService.GetMenuCategoryByIdAsync(model.MenuCategoryId);
                if (category == null || category.StoreId != store.Id)
                {
                    TempData["ErrorMessage"] = "Invalid category.";
                    return RedirectToAction("Index");
                }

                var menuItem = new MenuItem
                {
                    Name = model.Name,
                    Price = model.Price,
                    PointsRequired = model.PointsRequired,
                    MenuCategoryId = model.MenuCategoryId
                };

                await _menuItemService.AddMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Menu item added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                model.Categories = (await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id)).ToList();
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                CheckForErrorMessage();
                return View(model);
            }
        }

        // Edit Menu Item
        public async Task<IActionResult> EditMenuItem(int id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                TempData["ErrorMessage"] = "Menu item not found.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
            if (store == null || menuItem.MenuCategory.StoreId != store.Id)
            {
                TempData["ErrorMessage"] = "No store found for this owner or invalid menu item.";
                return RedirectToAction("Index");
            }

            var categories = await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id);
            var model = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                PointsRequired = menuItem.PointsRequired,
                MenuCategoryId = menuItem.MenuCategoryId,
                Categories = categories.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItem(MenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                model.Categories = (await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id)).ToList();
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var menuItem = await _menuItemService.GetMenuItemByIdAsync(model.Id);
                if (menuItem == null)
                {
                    TempData["ErrorMessage"] = "Menu item not found.";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null || menuItem.MenuCategory.StoreId != store.Id)
                {
                    TempData["ErrorMessage"] = "No store found for this owner or invalid menu item.";
                    return RedirectToAction("Index");
                }

                var category = await _menuCategoryService.GetMenuCategoryByIdAsync(model.MenuCategoryId);
                if (category == null || category.StoreId != store.Id)
                {
                    TempData["ErrorMessage"] = "Invalid category.";
                    return RedirectToAction("Index");
                }

                menuItem.Name = model.Name;
                menuItem.Price = model.Price;
                menuItem.PointsRequired = model.PointsRequired;
                menuItem.MenuCategoryId = model.MenuCategoryId;

                await _menuItemService.UpdateMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Menu item updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                model.Categories = (await _menuCategoryService.GetMenuCategoriesByStoreAsync(store.Id)).ToList();
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                CheckForErrorMessage();
                return View(model);
            }
        }

        // Delete Menu Item
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
                if (menuItem == null)
                {
                    TempData["ErrorMessage"] = "Menu item not found.";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null || menuItem.MenuCategory.StoreId != store.Id)
                {
                    TempData["ErrorMessage"] = "No store found for this owner or invalid menu item.";
                    return RedirectToAction("Index");
                }

                await _menuItemService.DeleteMenuItemAsync(id);
                TempData["SuccessMessage"] = "Menu item deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the menu item.";
            }
            return RedirectToAction("Index");
        }

        // Approve/Reject Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status, string rejectionReason)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null || order.Room.StoreId != store.Id)
                {
                    TempData["ErrorMessage"] = "No store found for this owner or invalid order.";
                    return RedirectToAction("Index");
                }

                order.Status = status;
                if (status == OrderStatus.Rejected)
                {
                    order.RejectionReason = rejectionReason;
                }

                await _orderService.UpdateOrderAsync(order);
                TempData["SuccessMessage"] = $"Order {status.ToString().ToLower()} successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while updating the order.";
            }
            return RedirectToAction("Index");
        }

        // Approve/Reject Assistance Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssistanceRequestStatus(int requestId, AssistanceRequestStatus status, string rejectionReason)
        {
            try
            {
                var request = await _assistanceRequestService.GetAssistanceRequestByIdAsync(requestId);
                if (request == null)
                {
                    TempData["ErrorMessage"] = "Assistance request not found.";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.GetUserAsync(User);
                var store = (await _storeService.FindStoreAsync(s => s.OwnerEmail == user.Email && !s.IsDeleted)).FirstOrDefault();
                if (store == null || request.Room.StoreId != store.Id)
                {
                    TempData["ErrorMessage"] = "No store found for this owner or invalid request.";
                    return RedirectToAction("Index");
                }

                request.Status = status;
                if (status == AssistanceRequestStatus.Rejected)
                {
                    request.RejectionReason = rejectionReason;
                }

                await _assistanceRequestService.UpdateAssistanceRequestAsync(request);
                TempData["SuccessMessage"] = $"Assistance request {status.ToString().ToLower()} successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while updating the assistance request.";
            }
            return RedirectToAction("Index");
        }
    }
}