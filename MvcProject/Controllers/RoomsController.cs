using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Domain.Models;
using System.Application.Abstraction;

namespace MvcProject.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IStoreService _storeService;
        private void CheckForErrorMessage()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"]?.ToString();
            }
        }

        public RoomsController(IRoomService roomService, IStoreService storeService)
        {
            _roomService = roomService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                return NotFound();
            }
            ViewBag.Store = store;

            var rooms = await _roomService.GetAllRoomsAsync(storeId, false);
            CheckForErrorMessage();
            return View(rooms);
        }

        public async Task<IActionResult> Deleted(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                return NotFound();
            }
            ViewBag.Store = store;

            var rooms = await _roomService.GetAllRoomsAsync(storeId, true);
            CheckForErrorMessage();
            return View(rooms);
        }

        public async Task<IActionResult> Create(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                return NotFound();
            }
            var room = new Room { StoreId = storeId };
            ViewBag.Store = store;
            CheckForErrorMessage();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (room.StoreId != 0)
            {
                await _roomService.AddRoomAsync(room);
                return RedirectToAction("Index", "Stores");
            }

            var store = await _storeService.GetStoreByIdAsync(room.StoreId);
            ViewBag.Store = store;
            CheckForErrorMessage();
            return View(room);
        }

        public async Task<IActionResult> Edit(int storeId, int roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null || room.StoreId != storeId)
            {
                return NotFound();
            }

            var store = await _storeService.GetStoreByIdAsync(storeId);
            ViewBag.Store = store;
            CheckForErrorMessage();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int storeId, int roomId, Room room)
        {
            if (roomId != room.Id || storeId != room.StoreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _roomService.UpdateRoomAsync(room);
                return RedirectToAction("Index", "Stores");
            }

            var store = await _storeService.GetStoreByIdAsync(storeId);
            ViewBag.Store = store;
            CheckForErrorMessage();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int storeId, int roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null || room.StoreId != storeId)
            {
                return NotFound();
            }

            await _roomService.DeleteRoomAsync(roomId);
            CheckForErrorMessage();
            return RedirectToAction("Index", "Stores");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int storeId, int roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null || room.StoreId != storeId)
            {
                return NotFound();
            }

            await _roomService.RestoreRoomAsync(roomId);
            CheckForErrorMessage();
            return RedirectToAction("Deleted", new { storeId });
        }
    }
}