using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;

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
        public async Task<ApiResponse<Order>> CreateOrderAsync(string phoneNumber, int roomId, List<(int menuItemId, int quantity)> items)
        {
            if (string.IsNullOrEmpty(phoneNumber) || roomId <= 0 || items == null || items.Count == 0)
                return new ApiResponse<Order>("رقم التليفون ، والأصناف يجب أن تكون صالحة", 400);


            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
            if (room == null)
                return new ApiResponse<Order>("الغرفة غير موجودة", 404);


            var customers = await _unitOfWork.GetRepository<Customer, int>().FindWithIncludesAsync(
                c => c.PhoneNumber == phoneNumber && c.StoreId == room.StoreId,
                includeDeleted: false);
            var customer = customers.FirstOrDefault();

            if (customer == null)
                return new ApiResponse<Order>("العميل غير مسجل، سجل برقم تليفونك أولاً", 400);


            var order = new Order
            {
                CustomerId = customer.Id,
                RoomId = roomId,
                Status = Status.Pending,
                OrderDate = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Order, int>().AddAsync(order);

            foreach (var item in items)
            {
                if (item.quantity <= 0 || item.menuItemId <= 0)
                    return new ApiResponse<Order>("كمية الصنف أو معرف الصنف غير موجود", 400);


                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = item.menuItemId,
                    Quantity = item.quantity,
                    CreatedOn = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<OrderItem, int>().AddAsync(orderItem);
                order.OrderItems.Add(orderItem);
            }

            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendOrderNotificationAsync(customer.StoreId, roomId);

            return new ApiResponse<Order>(order, "تم إضافة الطلب بنجاح", 201);
        }

        //* Get Pending Orders
        public async Task<ApiResponse<List<Order>>> GetPendingOrdersAsync(int storeId)
        {
            var orders = await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(
                predicate: o => o.Customer.StoreId == storeId && o.Status == Status.Pending,
                include: q => q.Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem),
                includeDeleted: false);

            if (!orders.Any())
                return new ApiResponse<List<Order>>("لا يوجد طلبات معلقة", 404);


            return new ApiResponse<List<Order>>(orders.ToList(), "تم جلب الطلبات المعلقة بنجاح");
        }

        //* Get All Orders
        public async Task<ApiResponse<List<Order>>> GetOrdersAsync(int storeId, bool includeDeleted = false)
        {
            var orders = await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(
                predicate: o => o.Customer.StoreId == storeId,
                include: q => q.Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem),
                includeDeleted: includeDeleted);

            if (!orders.Any())
                return new ApiResponse<List<Order>>("لا يوجد طلبات", 404);


            return new ApiResponse<List<Order>>(orders.ToList(), "تم جلب الطلبات بنجاح");
        }

        //* Approve Order
        public async Task<ApiResponse<Order>> ApproveOrderAsync(int orderId, decimal totalAmount)
        {
            if (orderId <= 0 || totalAmount < 0)
            {
                return new ApiResponse<Order>("المبلغ الإجمالي غير صحيح", 400);
            }

            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithIncludesAsync(
                orderId, include: q => q.Include(o => o.Customer).Include(o => o.Room));
            if (order == null)
                return new ApiResponse<Order>("الطلب غير موجود", 404);


            if (order.Status != Status.Pending)
                return new ApiResponse<Order>("لا يمكن الموافقة على طلب غير معلق", 400);


            order.Status = Status.Accepted;
            order.TotalAmount = totalAmount;
            order.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Order, int>().Update(order);

            var pointSettings = await _unitOfWork.GetRepository<PointSetting, int>().FindWithIncludesAsync(
                ps => ps.StoreId == order.Customer.StoreId,
                includeDeleted: false);
            var pointSetting = pointSettings.FirstOrDefault();

            if (pointSetting != null)
            {
                var points = (int)(totalAmount / pointSetting.Amount * pointSetting.Points);
                order.Customer.Points += points;
                _unitOfWork.GetRepository<Customer, int>().Update(order.Customer);
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
                orderId, include: q => q.Include(o => o.Room));
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
                o => o.Customer.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد الطلبات بنجاح");
        }

        #endregion
    }
}