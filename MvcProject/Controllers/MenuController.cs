using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using MvcProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Domain.Enums;

namespace MvcProject.Controllers
{
    [AllowAnonymous]
    public class MenuController : Controller
    {
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IAssistanceRequestService _assistanceRequestService;

        public MenuController(
            IMenuCategoryService menuCategoryService,
            IMenuItemService menuItemService,
            ICustomerService customerService,
            IOrderService orderService,
            IAssistanceRequestService assistanceRequestService)
        {
            _menuCategoryService = menuCategoryService;
            _menuItemService = menuItemService;
            _customerService = customerService;
            _orderService = orderService;
            _assistanceRequestService = assistanceRequestService;
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
            // Check if RoomId, StoreId, and CustomerId are in session
            if (!HttpContext.Session.TryGetValue("RoomId", out _) ||
                !HttpContext.Session.TryGetValue("StoreId", out _) ||
                !HttpContext.Session.TryGetValue("CustomerId", out _))
            {
                return RedirectToAction("Index", "RoomLogin");
            }

            int storeId = HttpContext.Session.GetInt32("StoreId") ?? 0;
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            try
            {
                // Get customer points
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null || customer.StoreId != storeId)
                {
                    return RedirectToAction("Index", "CustomerLogin");
                }

                // Get menu categories with their items for the store
                var categories = await _menuCategoryService.GetMenuCategoriesByStoreAsync(storeId);

                var viewModel = new MenuViewModel
                {
                    CustomerPoints = customer.Points,
                    Categories = categories.ToList()
                };

                CheckForErrorMessage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while loading the menu. Please try again later.";
                CheckForErrorMessage();
                return View(new MenuViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int menuItemId, bool usePoints)
        {
            // Check if RoomId, StoreId, and CustomerId are in session
            if (!HttpContext.Session.TryGetValue("RoomId", out _) ||
                !HttpContext.Session.TryGetValue("StoreId", out _) ||
                !HttpContext.Session.TryGetValue("CustomerId", out _))
            {
                return RedirectToAction("Index", "RoomLogin");
            }

            int storeId = HttpContext.Session.GetInt32("StoreId") ?? 0;
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            int roomId = HttpContext.Session.GetInt32("RoomId") ?? 0;

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null || customer.StoreId != storeId)
                {
                    return RedirectToAction("Index", "CustomerLogin");
                }

                var menuItem = await _menuItemService.GetMenuItemByIdAsync(menuItemId);
                if (menuItem == null || menuItem.MenuCategory.StoreId != storeId)
                {
                    TempData["ErrorMessage"] = "Invalid menu item.";
                    return RedirectToAction("Index");
                }

                // Check if using points
                int pointsUsed = 0;
                decimal totalAmount = menuItem.Price;

                if (usePoints)
                {
                    if (customer.Points < menuItem.PointsRequired)
                    {
                        TempData["ErrorMessage"] = "You do not have enough points to redeem this item.";
                        return RedirectToAction("Index");
                    }
                    pointsUsed = menuItem.PointsRequired;
                    totalAmount = 0; // No monetary cost when using points
                }

                // Create order
                var order = new Order
                {
                    CustomerId = customerId,
                    RoomId = roomId,
                    TotalAmount = totalAmount,
                    PaymentMethod = usePoints ? "Points" : "Cash",
                    PointsUsed = pointsUsed,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { MenuItemId = menuItemId }
                    }
                };

                await _orderService.AddOrderAsync(order);

                TempData["SuccessMessage"] = "Order placed successfully! Waiting for approval.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while placing the order. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult RequestAssistance()
        {
            // Check if RoomId, StoreId, and CustomerId are in session
            if (!HttpContext.Session.TryGetValue("RoomId", out _) ||
                !HttpContext.Session.TryGetValue("StoreId", out _) ||
                !HttpContext.Session.TryGetValue("CustomerId", out _))
            {
                return RedirectToAction("Index", "RoomLogin");
            }

            return View(new AssistanceRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestAssistance(AssistanceRequestViewModel model)
        {
            // Check if RoomId, StoreId, and CustomerId are in session
            if (!HttpContext.Session.TryGetValue("RoomId", out _) ||
                !HttpContext.Session.TryGetValue("StoreId", out _) ||
                !HttpContext.Session.TryGetValue("CustomerId", out _))
            {
                return RedirectToAction("Index", "RoomLogin");
            }

            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            int roomId = HttpContext.Session.GetInt32("RoomId") ?? 0;

            try
            {
                var assistanceRequest = new AssistanceRequest
                {
                    CustomerId = customerId,
                    RoomId = roomId,
                    RequestType = model.RequestType,
                    Status = AssistanceRequestStatus.Pending,
                    RequestDate = DateTime.UtcNow
                };

                await _assistanceRequestService.AddAssistanceRequestAsync(assistanceRequest);

                TempData["SuccessMessage"] = "Assistance request submitted successfully! Waiting for approval.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while submitting the request. Please try again later.";
                CheckForErrorMessage();
                return View(model);
            }
        }
    }
}