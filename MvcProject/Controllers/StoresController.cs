using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Domain.Models;
using System.Application.Abstraction;

namespace MvcProject.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IRoomService _roomService;

        private void CheckForErrorMessage()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"]?.ToString();
            }
        }

        public StoresController(IStoreService storeService, IRoomService roomService)
        {
            _storeService = storeService;
            _roomService = roomService;
        }

        public async Task<IActionResult> Index()
        {
            var stores = await _storeService.GetAllStoresAsync(false);

            foreach (var store in stores)
            {
                var rooms = await _roomService.GetAllRoomsAsync(store.Id, false);
                ViewData[$"Rooms_{store.Id}"] = rooms;
            }
            CheckForErrorMessage();
            return View(stores);
        }

        public async Task<IActionResult> Deleted()
        {
            var stores = await _storeService.GetAllStoresAsync(true);

            foreach (var store in stores)
            {
                var rooms = await _roomService.GetAllRoomsAsync(store.Id, true);
                ViewData[$"Rooms_{store.Id}"] = rooms;
            }
            CheckForErrorMessage();
            return View(stores);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Store store)
        {
            if (ModelState.IsValid)
            {
                await _storeService.AddStoreAsync(store);
                return RedirectToAction(nameof(Index));
            }
            CheckForErrorMessage();
            return View(store);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _storeService.UpdateStoreAsync(store);
                return RedirectToAction(nameof(Index));
            }
            CheckForErrorMessage();
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _storeService.DeleteStoreAsync(id);
            CheckForErrorMessage();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _storeService.RestoreStoreAsync(id);
            CheckForErrorMessage();
            return RedirectToAction(nameof(Deleted));
        }
    }
}