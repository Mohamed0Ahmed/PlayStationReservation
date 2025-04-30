using Microsoft.AspNetCore.Mvc;
using System.Application.Abstraction;
using Microsoft.AspNetCore.Authorization;
using MvcProject.Models;

namespace MvcProject.Controllers
{
    [AllowAnonymous]
    public class RoomLoginController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IStoreService _storeService;

        public RoomLoginController(IRoomService roomService, IStoreService storeService)
        {
            _roomService = roomService;
            _storeService = storeService;
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
            return View(new RoomLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RoomLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                // Find the store by name
                var store =  await _storeService.GetStoreByNameAsync(model.StoreName);
                if (store == null)
                {
                    ModelState.AddModelError(string.Empty, "Store not found.");
                    CheckForErrorMessage();
                    return View(model);
                }

                // Find the room by username and password within the store
                var room = await _roomService.GetRoomByNameAsync(model.Username , model.Password , store.Id);
                if (room == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password for the specified store.");
                    CheckForErrorMessage();
                    return View(model);
                }

                // Store RoomId and StoreId in session
                HttpContext.Session.SetInt32("RoomId", room.Id);
                HttpContext.Session.SetInt32("StoreId", store.Id);

                return RedirectToAction("Index", "CustomerLogin");
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
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}