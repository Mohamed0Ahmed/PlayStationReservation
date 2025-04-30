using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using System.Domain.Models;
using MvcProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace MvcProject.Controllers
{
    [AllowAnonymous]
    public class CustomerLoginController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerLoginController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private void CheckForErrorMessage()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"]?.ToString();
            }
        }

        public IActionResult Index()
        {
            // Check if RoomId and StoreId are in session (iPad must be logged in)
            if (!HttpContext.Session.TryGetValue("RoomId", out _) || !HttpContext.Session.TryGetValue("StoreId", out _))
            {
                return RedirectToAction("Index", "RoomLogin");
            }

            return View(new CustomerLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CustomerLoginViewModel model)
        {
            // Check if RoomId and StoreId are in session
            if (!HttpContext.Session.TryGetValue("RoomId", out _) || !HttpContext.Session.TryGetValue("StoreId", out _))
            {
                return RedirectToAction("Index", "RoomLogin");
            }

            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                // Get StoreId from session
                int storeId = HttpContext.Session.GetInt32("StoreId") ?? 0;

                // Check if customer exists for this phone number and store
                var customer = await _customerService.GetCustomerByPhoneAsync(model.PhoneNumber , storeId);
                if (customer == null)
                {
                    // Create new customer if not exists for this store
                    customer = new Customer
                    {
                        PhoneNumber = model.PhoneNumber,
                        StoreId = storeId,
                        Points = 0
                    };
                    await _customerService.AddCustomerAsync(customer);
                }

                // Store CustomerId in session
                HttpContext.Session.SetInt32("CustomerId", customer.Id);

                return RedirectToAction("Index", "Menu");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CheckForErrorMessage();
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index");
        }
    }
}