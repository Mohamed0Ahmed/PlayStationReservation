using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Domain.Models;
using System.Application.Abstraction;
using MvcProject.Models;

namespace MvcProject.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IStoreService _storeService;

        public RoomsController(IRoomService roomService, IStoreService storeService)
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                return NotFound();
            }

            var roomViewModel = new RoomViewModel { StoreId = storeId };
            ViewBag.Store = store;
            return View(roomViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RoomViewModel roomViewModel)
        {
            if (!ModelState.IsValid)
            {
                var store = await _storeService.GetStoreByIdAsync(roomViewModel.StoreId);
                ViewBag.Store = store;
                CheckForErrorMessage();
                return View(roomViewModel);
            }

            try
            {
                var room = new Room
                {
                    StoreId = roomViewModel.StoreId,
                    Username = roomViewModel.Username,
                    Password = roomViewModel.Password
                };
                await _roomService.AddRoomAsync(room);
                return RedirectToAction("ViewRooms", "Stores", new { storeId = roomViewModel.StoreId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to create room: {ex.Message}";
                var store = await _storeService.GetStoreByIdAsync(roomViewModel.StoreId);
                ViewBag.Store = store;
                CheckForErrorMessage();
                return View(roomViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int storeId, int roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null || room.StoreId != storeId)
            {
                return NotFound();
            }

            var roomViewModel = new RoomViewModel
            {
                Id = room.Id,
                StoreId = room.StoreId,
                Username = room.Username,
                Password = room.Password
            };

            var store = await _storeService.GetStoreByIdAsync(storeId);
            ViewBag.Store = store;
            CheckForErrorMessage();
            return View(roomViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int storeId, int roomId, RoomViewModel roomViewModel)
        {
            if (roomId != roomViewModel.Id || storeId != roomViewModel.StoreId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var store = await _storeService.GetStoreByIdAsync(storeId);
                ViewBag.Store = store;
                CheckForErrorMessage();
                return View(roomViewModel);
            }

            try
            {
                var room = new Room
                {
                    Id = roomViewModel.Id,
                    StoreId = roomViewModel.StoreId,
                    Username = roomViewModel.Username,
                    Password = roomViewModel.Password
                };
                await _roomService.UpdateRoomAsync(room);
                return RedirectToAction("ViewRooms", "Stores", new { storeId = roomViewModel.StoreId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update room: {ex.Message}";
                var store = await _storeService.GetStoreByIdAsync(storeId);
                ViewBag.Store = store;
                CheckForErrorMessage();
                return View(roomViewModel);
            }
        }
    }


}