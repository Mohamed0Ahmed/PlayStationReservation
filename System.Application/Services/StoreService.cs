using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store, int> _storeRepository;
        private readonly IRepository<Room, int> _roomRepository;
        private readonly IRepository<PointSetting, int> _pointSettingRepository;

        public StoreService(
            IRepository<Store, int> storeRepository,
            IRepository<Room, int> roomRepository,
            IRepository<PointSetting, int> pointSettingRepository)
        {
            _storeRepository = storeRepository;
            _roomRepository = roomRepository;
            _pointSettingRepository = pointSettingRepository;
        }

        public async Task<ApiResponse<Store>> CreateStoreAsync(string name, string ownerEmail)
        {
            var store = new Store
            {
                Name = name,
                OwnerEmail = ownerEmail,
                CreatedOn = DateTime.UtcNow
            };

            await _storeRepository.AddAsync(store);
            return new ApiResponse<Store>(store, "تم إضافة المحل بنجاح", 201);
        }

        public async Task<ApiResponse<Store>> UpdateStoreAsync(int storeId, string name, string ownerEmail)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<Store>("المحل غير موجود", 404);
            }

            store.Name = name;
            store.OwnerEmail = ownerEmail;
            store.LastModifiedOn = DateTime.UtcNow;
            _storeRepository.Update(store);

            return new ApiResponse<Store>(store, "تم تعديل المحل بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteStoreAsync(int storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<bool>("المحل غير موجود", 404);
            }

            _storeRepository.Delete(store);
            return new ApiResponse<bool>(true, "تم حذف المحل بنجاح");
        }

        public async Task<ApiResponse<List<Store>>> GetStoresAsync()
        {
            var stores = await _storeRepository.GetAllAsync();
            return new ApiResponse<List<Store>>(stores.ToList());
        }

        public async Task<ApiResponse<Room>> CreateRoomAsync(int storeId, string username, string password)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<Room>("المحل غير موجود", 404);
            }

            var room = new Room
            {
                StoreId = storeId,
                Username = username,
                Password = password,
                CreatedOn = DateTime.UtcNow
            };

            await _roomRepository.AddAsync(room);
            return new ApiResponse<Room>(room, "تم إضافة الغرفة بنجاح", 201);
        }

        public async Task<ApiResponse<Room>> UpdateRoomAsync(int roomId, string username, string password)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                return new ApiResponse<Room>("الغرفة غير موجودة", 404);
            }

            room.Username = username;
            room.Password = password;
            room.LastModifiedOn = DateTime.UtcNow;
            _roomRepository.Update(room);

            return new ApiResponse<Room>(room, "تم تعديل الغرفة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteRoomAsync(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                return new ApiResponse<bool>("الغرفة غير موجودة", 404);
            }

            _roomRepository.Delete(room);
            return new ApiResponse<bool>(true, "تم حذف الغرفة بنجاح");
        }

        public async Task<ApiResponse<List<Room>>> GetRoomsAsync(int storeId)
        {
            var rooms = await _roomRepository.FindAsync(r => r.StoreId == storeId);
            return new ApiResponse<List<Room>>(rooms.ToList());
        }

        public async Task<ApiResponse<PointSetting>> CreatePointSettingAsync(int storeId, decimal amountPerPoint, int points)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<PointSetting>("المحل غير موجود", 404);
            }

            var setting = new PointSetting
            {
                StoreId = storeId,
                Amount = amountPerPoint,
                Points = points,
                CreatedOn = DateTime.UtcNow
            };

            await _pointSettingRepository.AddAsync(setting);
            return new ApiResponse<PointSetting>(setting, "تم إضافة إعداد النقاط بنجاح", 201);
        }

        public async Task<ApiResponse<PointSetting>> UpdatePointSettingAsync(int settingId, decimal amountPerPoint, int points)
        {
            var setting = await _pointSettingRepository.GetByIdAsync(settingId);
            if (setting == null)
            {
                return new ApiResponse<PointSetting>("إعداد النقاط غير موجود", 404);
            }

            setting.Amount = amountPerPoint;
            setting.Points = points;
            setting.LastModifiedOn = DateTime.UtcNow;
            _pointSettingRepository.Update(setting);

            return new ApiResponse<PointSetting>(setting, "تم تعديل إعداد النقاط بنجاح");
        }

        public async Task<ApiResponse<bool>> DeletePointSettingAsync(int settingId)
        {
            var setting = await _pointSettingRepository.GetByIdAsync(settingId);
            if (setting == null)
            {
                return new ApiResponse<bool>("إعداد النقاط غير موجود", 404);
            }

            _pointSettingRepository.Delete(setting);
            return new ApiResponse<bool>(true, "تم حذف إعداد النقاط بنجاح");
        }

        public async Task<ApiResponse<List<PointSetting>>> GetPointSettingsAsync(int storeId)
        {
            var settings = await _pointSettingRepository.FindAsync(ps => ps.StoreId == storeId);
            return new ApiResponse<List<PointSetting>>(settings.ToList());
        }

        public async Task<ApiResponse<int>> GetTotalStoresCountAsync()
        {
            var count = (await _storeRepository.GetAllAsync()).Count();
            return new ApiResponse<int>(count, "تم جلب عدد المحلات بنجاح");
        }
    }
}