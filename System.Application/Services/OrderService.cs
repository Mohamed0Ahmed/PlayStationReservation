using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.Orders;

namespace System.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public OrderService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        #region Orders

        //* Create Order
        public async Task<ApiResponse<Order>> CreateOrderAsync(string phoneNumber, int roomId, List<ItemsDto> items)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
            if (room == null)
                return new ApiResponse<Order>("الغرفة غير موجودة", 404);

            var customers = await _unitOfWork.GetRepository<Customer, int>().FindAsync(
                c => c.PhoneNumber == phoneNumber && c.StoreId == room.StoreId);
            var customer = customers.FirstOrDefault();

            if (customer == null)
                return new ApiResponse<Order>("لعميل غير مسجل، سجل برقم تليفونك أولا", 400);

            var order = new Order
            {
                StoreId = customer.StoreId,
                CustomerId = customer.Id,
                RoomId = roomId,
                Status = Status.Pending,
                OrderDate = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            foreach (var item in items)
            {
                var menuItem = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(item.MenuItemId);
                if (menuItem == null)
                    return new ApiResponse<Order>("هذا الصنف غير موجود", 404);

                var orderItem = new OrderItem
                {
                    PriceAtOrderTime = menuItem.Price,
                    OrderId = order.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    CreatedOn = DateTime.UtcNow
                };
                order.TotalAmount += (menuItem.Price * item.Quantity);

                await _unitOfWork.GetRepository<OrderItem, int>().AddAsync(orderItem);
                order.OrderItems.Add(orderItem);
            }

            await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendOrderNotificationAsync(customer.StoreId, roomId);

            return new ApiResponse<Order>(order, "تم إضافة الطلب بنجاح", 201);
        }

        //* Get Pending Orders
        public async Task<ApiResponse<List<Order>>> GetPendingOrdersAsync(int storeId)
        {
            var orders = await _unitOfWork.GetRepository<Order, int>().FindAsync(o => o.StoreId == storeId && o.Status == Status.Pending);

            if (!orders.Any())
                return new ApiResponse<List<Order>>("لا يوجد طلبات معلقة", 404);


            return new ApiResponse<List<Order>>(orders.ToList(), "تم جلب الطلبات المعلقة بنجاح");
        }

        //* Get All Orders
        public async Task<ApiResponse<List<Order>>> GetOrdersAsync(int storeId, bool includeDeleted = false)
        {
            var orders = await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(
                predicate: o => o.StoreId == storeId);

            if (!orders.Any())
                return new ApiResponse<List<Order>>("لا يوجد طلبات", 404);


            return new ApiResponse<List<Order>>(orders.ToList(), "تم جلب الطلبات بنجاح");
        }

        //* Approve Order
        public async Task<ApiResponse<Order>> ApproveOrderAsync(int orderId)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(orderId);
            if (order == null)
                return new ApiResponse<Order>("الطلب غير موجود", 404);

            if (order.Status != Status.Pending)
                return new ApiResponse<Order>("لا يمكن الموافقة على طلب غير معلق", 400);

            var customers = await _unitOfWork.GetRepository<Customer, int>().FindAsync(c => c.Id == order.CustomerId);
            var customer = customers.FirstOrDefault();
            if (customer == null)
                return new ApiResponse<Order>(order, "لا يوجد عميل بهذا الرقم");


            order.Status = Status.Accepted;
            order.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Order, int>().Update(order);

            var pointSettings = await _unitOfWork.GetRepository<PointSetting, int>().FindAsync(ps => ps.StoreId == order.StoreId);
            var pointSetting = pointSettings.FirstOrDefault();

            if (pointSetting != null)
            {
                var points = (int)(order.TotalAmount / pointSetting.Amount * pointSetting.Points);


                customer.Points += points;
                _unitOfWork.GetRepository<Customer, int>().Update(customer);
            }

            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendOrderStatusUpdateAsync(order.RoomId, true);

            return new ApiResponse<Order>(order, "تم الموافقة على الطلب بنجاح");
        }

        //* Reject Order
        public async Task<ApiResponse<Order>> RejectOrderAsync(int orderId, string rejectionReason)
        {
            if (orderId <= 0 || string.IsNullOrEmpty(rejectionReason))
                return new ApiResponse<Order>("  سبب الرفض غير موجود", 400);


            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithIncludesAsync(
                orderId);
            if (order == null)
                return new ApiResponse<Order>("الطلب غير موجود", 404);


            if (order.Status != Status.Pending)
                return new ApiResponse<Order>("لا يمكن رفض طلب غير معلق", 400);


            order.Status = Status.Rejected;
            order.RejectionReason = rejectionReason;
            order.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Order, int>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendOrderStatusUpdateAsync(order.RoomId, false, rejectionReason);
            return new ApiResponse<Order>(order, "تم رفض الطلب بنجاح");
        }

        //* Get Total Orders Count
        public async Task<ApiResponse<int>> GetTotalOrdersCountAsync(int storeId)
        {
            var count = (await _unitOfWork.GetRepository<Order, int>().FindAsync(
                o => o.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد الطلبات بنجاح");
        }

        #endregion
    }
}