using Mapster;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ApiResponse<Order>> CreateOrderAsync(string phoneNumber, int roomId, IEnumerable<ItemsDto> items)
        {
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
            if (room == null)
                return new ApiResponse<Order>("الغرفة غير موجودة", 200);

            var customers = await _unitOfWork.GetRepository<Customer, int>().FindAsync(
                c => c.PhoneNumber == phoneNumber && c.StoreId == room.StoreId);
            var customer = customers.FirstOrDefault();

            if (customer == null)
                return new ApiResponse<Order>("العميل غير مسجل، سجل برقم تليفونك أولا", 200);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    RoomName = room.Username,
                    CustomerNumber = customer.PhoneNumber,
                    StoreId = customer.StoreId,
                    CustomerId = customer.Id,
                    RoomId = roomId,
                    Status = Status.Pending,
                    OrderDate = DateTime.UtcNow,
                    CreatedOn = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>(),
                    TotalAmount = 0
                };

                await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                foreach (var item in items)
                {
                    var menuItem = await _unitOfWork.GetRepository<MenuItem, int>().GetByIdAsync(item.MenuItemId);
                    if (menuItem == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return new ApiResponse<Order>("هذا الصنف غير موجود", 200);
                    }

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

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _notificationService.SendOrderNotificationAsync(customer.StoreId, roomId);

                return new ApiResponse<Order>(order, "تم إضافة الطلب بنجاح", 201);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<Order>($"حدث خطأ أثناء تنفيذ الطلب: {ex.Message}", 500);
            }
        }



        //* Get Pending Orders
        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetPendingOrdersAsync(int storeId)
        {
            var orders = await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(o => o.StoreId == storeId && o.Status == Status.Pending , include: o=>o.Include(o=>o.OrderItems));

            if (!orders.Any())
                return new ApiResponse<IEnumerable<OrderDto>>("لا يوجد طلبات معلقة", 200);

            var orderDto = orders.Adapt<List<OrderDto>>();

            return new ApiResponse<IEnumerable<OrderDto>>(orderDto, "تم جلب الطلبات المعلقة بنجاح");
        }



        //* Get All Orders
        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersAsync(int storeId, bool includeDeleted = false)
        {
            var orders = await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(
                predicate: o => o.StoreId == storeId , include: o => o.Include(o => o.OrderItems));


            if (!orders.Any())
                return new ApiResponse<IEnumerable<OrderDto>>("لا يوجد طلبات", 200);

            var orderDto = orders.Adapt<List<OrderDto>>();
            return new ApiResponse<IEnumerable<OrderDto>>(orderDto, "تم جلب الطلبات بنجاح");
        }



        //* Approve Order
        public async Task<ApiResponse<OrderDto>> ApproveOrderAsync(int orderId)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(orderId);
            if (order == null)
                return new ApiResponse<OrderDto>("الطلب غير موجود", 200);

            if (order.Status != Status.Pending)
                return new ApiResponse<OrderDto>("لا يمكن الموافقة على طلب غير معلق", 200);

            var customers = await _unitOfWork.GetRepository<Customer, int>().FindAsync(c => c.Id == order.CustomerId);
            var customer = customers.FirstOrDefault();
            if (customer == null)
                return new ApiResponse<OrderDto>( "لا يوجد عميل بهذا الرقم");


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

            var orderDto = order.Adapt<OrderDto>();
            return new ApiResponse<OrderDto>(orderDto, "تم الموافقة على الطلب بنجاح");
        }



        //* Reject Order
        public async Task<ApiResponse<OrderDto>> RejectOrderAsync(int orderId, string rejectionReason)
        {
            if (orderId <= 0 || string.IsNullOrEmpty(rejectionReason))
                return new ApiResponse<OrderDto>("  سبب الرفض غير موجود", 200);


            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithIncludesAsync(
                orderId);
            if (order == null)
                return new ApiResponse<OrderDto>("الطلب غير موجود", 200);


            if (order.Status != Status.Pending)
                return new ApiResponse<OrderDto>("لا يمكن رفض طلب غير معلق", 200);


            order.Status = Status.Rejected;
            order.RejectionReason = rejectionReason;
            order.LastModifiedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Order, int>().Update(order);
            await _unitOfWork.SaveChangesAsync();
            var orderDto = order.Adapt<OrderDto>();

            await _notificationService.SendOrderStatusUpdateAsync(order.RoomId, false, rejectionReason);
            return new ApiResponse<OrderDto>(orderDto, "تم رفض الطلب بنجاح");
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