using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;

namespace System.Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Stores

        //* create
        public async Task<ApiResponse<Store>> CreateStoreAsync(string name, string ownerEmail)
        {
            // Check if OwnerEmail is already in use
            if (await _unitOfWork.GetRepository<Store, int>().AnyAsync(s => s.OwnerEmail == ownerEmail || s.Name == name))
                return new ApiResponse<Store>("الإيميل او الاسم موجود بالفعل", 400);


            var store = new Store
            {
                Name = name,
                OwnerEmail = ownerEmail,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Store, int>().AddAsync(store);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<Store>(store, $"تم إضافة {nameof(Store)} بنجاح", 201);
        }

        //* update
        public async Task<ApiResponse<Store>> UpdateStoreAsync(int storeId, string name, string ownerEmail)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<Store>("المحل غير موجود", 404);
            }

            // Check if OwnerEmail is already in use by another store
            if (await _unitOfWork.GetRepository<Store, int>().AnyAsync(s => s.OwnerEmail == ownerEmail || s.Name == name))
                return new ApiResponse<Store>("الإيميل او الاسم موجود بالفعل", 400);

            store.Name = name;
            store.OwnerEmail = ownerEmail;
            store.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Store, int>().Update(store);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Store>(store, "تم تعديل المحل بنجاح");
        }

        //* update
        public async Task<ApiResponse<bool>> DeleteStoreAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<bool>("المحل غير موجود", 404);
            }

            var rooms = await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == store.Id);

            foreach (var room in rooms)
                _unitOfWork.GetRepository<Room, int>().Delete(room);

            _unitOfWork.GetRepository<Store, int>().Delete(store);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف المحل بنجاح");
        }

        //* restore
        public async Task<ApiResponse<bool>> RestoreStoreAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId, onlyDeleted:true);
            if (!store.IsDeleted)
            {
                return new ApiResponse<bool>("المحل غير موجود", 404);
            }

            var rooms = await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == store.Id , onlyDeleted:true);

            foreach (var room in rooms)
                await _unitOfWork.GetRepository<Room, int>().RestoreAsync(room.Id);

            await _unitOfWork.GetRepository<Store, int>().RestoreAsync(store.Id);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع المحل بنجاح");
        }


        //* get All
        public async Task<ApiResponse<List<Store>>> GetStoresAsync()
        {
            var stores = await _unitOfWork.GetRepository<Store, int>().GetAllAsync();
            if (!stores.Any())
                return new ApiResponse<List<Store>>("No Stores Added Yet", 404);

            return new ApiResponse<List<Store>>(stores.ToList());
        }


        //* get all deleted
        public async Task<ApiResponse<List<Store>>> GetDeletedStoresAsync()
        {
            var stores = await _unitOfWork.GetRepository<Store, int>().GetAllAsync(onlyDeleted: true);
            if (!stores.Any())
                return new ApiResponse<List<Store>>("No Deleted Stores", 404);

            return new ApiResponse<List<Store>>(stores.ToList());
        }


        #endregion



        #region Rooms

        //* Create
        public async Task<ApiResponse<Room>> CreateRoomAsync(int storeId, string username, string password)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
            {
                return new ApiResponse<Room>("المحل غير موجود", 404);
            }

            // Check if a room with the same Username exists in the same Store
            if (await _unitOfWork.GetRepository<Room, int>().AnyAsync(r => r.Username == username && r.StoreId == storeId))
            {
                return new ApiResponse<Room>("اسم المستخدم موجود بالفعل في هذا المحل", 400);
            }

            var room = new Room
            {
                StoreId = storeId,
                Username = username,
                Password = password,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Room, int>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<Room>(room, "تم إضافة الغرفة بنجاح", 201);
        }

        //* Update
        public async Task<ApiResponse<Room>> UpdateRoomAsync(int roomId, string username, string password)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
            if (room == null)
            {
                return new ApiResponse<Room>("الغرفة غير موجودة", 404);
            }

            // Check if another room with the same Username exists in the same Store
            if (await _unitOfWork.GetRepository<Room, int>().AnyAsync(r => r.Username == username && r.StoreId == room.StoreId && r.Id != roomId))
            {
                return new ApiResponse<Room>("اسم المستخدم موجود بالفعل في هذا المحل", 400);
            }

            room.Username = username;
            room.Password = password;
            room.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Room, int>().Update(room);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Room>(room, "تم تعديل الغرفة بنجاح");
        }

        //* Delete
        public async Task<ApiResponse<bool>> DeleteRoomAsync(int roomId)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
            if (room == null)
            {
                return new ApiResponse<bool>("الغرفة غير موجودة", 404);
            }

            _unitOfWork.GetRepository<Room, int>().Delete(room);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<bool>(true, "تم حذف الغرفة بنجاح");
        }

        //* Restore
        public async Task<ApiResponse<bool>> RestoreRoomAsync(int roomId)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId, onlyDeleted: true);
            if (room == null || !room.IsDeleted)
            {
                return new ApiResponse<bool>("الغرفة غير موجودة أو غير محذوفة", 404);
            }

            await _unitOfWork.GetRepository<Room, int>().RestoreAsync(room.Id);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع الغرفة بنجاح");
        }

        //* get all rooms
        public async Task<ApiResponse<List<Room>>> GetRoomsAsync(int storeId)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
            if (store == null)
                return new ApiResponse<List<Room>>("هذا المحل غير متواجد", 404);

            var rooms = await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == storeId);
            if (!rooms.Any())
                return new ApiResponse<List<Room>>("لا يوجد غرف لهذا المحل حاليا", 404);
            return new ApiResponse<List<Room>>(rooms.ToList());
        }

        #endregion



        #region Point settings

        public async Task<ApiResponse<PointSetting>> CreatePointSettingAsync(int storeId, decimal amountPerPoint, int points)
        {
            var store = await _unitOfWork.GetRepository<Store, int>().GetByIdAsync(storeId);
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

            await _unitOfWork.GetRepository<PointSetting, int>().AddAsync(setting);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<PointSetting>(setting, "تم إضافة إعداد النقاط بنجاح", 201);
        }

        public async Task<ApiResponse<PointSetting>> UpdatePointSettingAsync(int settingId, decimal amountPerPoint, int points)
        {
            var setting = await _unitOfWork.GetRepository<PointSetting, int>().GetByIdAsync(settingId);
            if (setting == null)
            {
                return new ApiResponse<PointSetting>("إعداد النقاط غير موجود", 404);
            }

            setting.Amount = amountPerPoint;
            setting.Points = points;
            setting.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<PointSetting, int>().Update(setting);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<PointSetting>(setting, "تم تعديل إعداد النقاط بنجاح");
        }

        public async Task<ApiResponse<bool>> DeletePointSettingAsync(int settingId)
        {
            var setting = await _unitOfWork.GetRepository<PointSetting, int>().GetByIdAsync(settingId);
            if (setting == null)
            {
                return new ApiResponse<bool>("إعداد النقاط غير موجود", 404);
            }

            _unitOfWork.GetRepository<PointSetting, int>().Delete(setting);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<bool>(true, "تم حذف إعداد النقاط بنجاح");
        }

        public async Task<ApiResponse<List<PointSetting>>> GetPointSettingsAsync(int storeId)
        {
            var settings = await _unitOfWork.GetRepository<PointSetting, int>().FindAsync(ps => ps.StoreId == storeId);
            return new ApiResponse<List<PointSetting>>(settings.ToList());
        } 

        #endregion


        public async Task<ApiResponse<int>> GetTotalStoresCountAsync()
        {
            var count = (await _unitOfWork.GetRepository<Store, int>().GetAllAsync()).Count();
            return new ApiResponse<int>(count, "تم جلب عدد المحلات بنجاح");
        }
    }
}