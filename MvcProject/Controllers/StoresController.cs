using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Domain.Models;
using System.Application.Abstraction;
using MvcProject.Models;

namespace MvcProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StoresController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IRoomService _roomService;
        private readonly ILogger<StoresController> _logger;

        public StoresController(IStoreService storeService, IRoomService roomService, ILogger<StoresController> logger)
        {
            _storeService = storeService;
            _roomService = roomService;
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
                var stores = await _storeService.GetAllStoresAsync(false);
                var storeViewModels = stores.Select(store => new StoreViewModel
                {
                    Id = store.Id,
                    Name = store.Name,
                    OwnerEmail = store.OwnerEmail,
                }).ToList();

                CheckForErrorMessage();
                return View(storeViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving active stores.");
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<StoreViewModel>());
            }
        }

        public async Task<IActionResult> ViewRooms(int storeId)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store == null)
                {
                    TempData["ErrorMessage"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                var activeRooms = await _roomService.GetAllRoomsAsync(storeId, false);
                var deletedRooms = await _roomService.GetAllRoomsAsync(storeId, true);

                var viewModel = new StoreRoomsViewModel
                {
                    StoreId = store.Id,
                    StoreName = store.Name,
                    ActiveRooms = activeRooms.ToList(),
                    DeletedRooms = deletedRooms.ToList()
                };

                CheckForErrorMessage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving rooms for store {StoreId}.", storeId);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Deleted()
        {
            try
            {
                var stores = await _storeService.GetAllStoresAsync(true);
                var storeViewModels = stores.Select(store => new StoreViewModel
                {
                    Id = store.Id,
                    Name = store.Name,
                    OwnerEmail = store.OwnerEmail,
                    Rooms = store.Rooms?.ToList() ?? new List<Room>()
                }).ToList();

                CheckForErrorMessage();
                return View(storeViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving deleted stores.");
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<StoreViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View(new StoreViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var store = new Store
                {
                    Name = model.Name,
                    OwnerEmail = model.OwnerEmail
                };
                await _storeService.AddStoreAsync(store);
                _logger.LogInformation("Store {StoreName} created successfully.", store.Name);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating store {StoreName}.", model.Name);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                var model = new StoreViewModel
                {
                    Id = store.Id,
                    Name = store.Name,
                    OwnerEmail = store.OwnerEmail
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving store {StoreId} for editing.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreViewModel model)
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
                var store = new Store
                {
                    Id = model.Id,
                    Name = model.Name,
                    OwnerEmail = model.OwnerEmail
                };
                await _storeService.UpdateStoreAsync(store);
                _logger.LogInformation("Store {StoreId} updated successfully.", store.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating store {StoreId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _storeService.DeleteStoreAsync(id);
                _logger.LogInformation("Store {StoreId} deleted successfully.", id);
                CheckForErrorMessage();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting store {StoreId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(int storeId, int roomId)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(roomId);
                if (room == null || room.StoreId != storeId)
                {
                    TempData["ErrorMessage"] = "Room not found or does not belong to the specified store.";
                    return RedirectToAction(nameof(ViewRooms), new { storeId });
                }

                await _roomService.DeleteRoomAsync(roomId);
                _logger.LogInformation("Room {RoomId} deleted successfully from Store {StoreId}.", roomId, storeId);
                CheckForErrorMessage();
                return RedirectToAction(nameof(ViewRooms), new { storeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting room {RoomId} from store {StoreId}.", roomId, storeId);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return RedirectToAction(nameof(ViewRooms), new { storeId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreRoom(int storeId, int roomId)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(roomId, includeDeleted: true);
                if (room == null || room.StoreId != storeId)
                {
                    TempData["ErrorMessage"] = "Room not found or does not belong to the specified store.";
                    return RedirectToAction(nameof(ViewRooms), new { storeId });
                }

                if (!room.IsDeleted)
                {
                    TempData["ErrorMessage"] = "Cannot restore a room that is not deleted.";
                    return RedirectToAction(nameof(ViewRooms), new { storeId });
                }

                await _roomService.RestoreRoomAsync(roomId);
                _logger.LogInformation("Room {RoomId} restored successfully for Store {StoreId}.", roomId, storeId);
                CheckForErrorMessage();
                return RedirectToAction(nameof(ViewRooms), new { storeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while restoring room {RoomId} for store {StoreId}.", roomId, storeId);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return RedirectToAction(nameof(ViewRooms), new { storeId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _storeService.RestoreStoreAsync(id);
                _logger.LogInformation("Store {StoreId} restored successfully.", id);
                CheckForErrorMessage();
                return RedirectToAction(nameof(Deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while restoring store {StoreId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                CheckForErrorMessage();
                return RedirectToAction(nameof(Deleted));
            }
        }
    }


}