using Microsoft.EntityFrameworkCore;
using System.Application.Abstraction;
using System.Domain.Enums;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<OrderItem, int> _orderItemRepository;
        private readonly IRepository<Customer, int> _customerRepository;
        private readonly IRepository<PointSetting, int> _pointSettingRepository;
        private readonly INotificationService _notificationService;

        public OrderService(
            IRepository<Order, int> orderRepository,
            IRepository<OrderItem, int> orderItemRepository,
            IRepository<Customer, int> customerRepository,
            IRepository<PointSetting, int> pointSettingRepository,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _customerRepository = customerRepository;
            _pointSettingRepository = pointSettingRepository;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<Order>> CreateOrderAsync(int customerId, int roomId, List<(int menuItemId, int quantity)> items)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiResponse<Order>("الزبون غير موجود", 404);
            }

            var order = new Order
            {
                CustomerId = customerId,
                RoomId = roomId,
                Status = 0,
                OrderDate = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);

            foreach (var item in items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = item.menuItemId,
                    Quantity = item.quantity,
                    CreatedOn = DateTime.UtcNow
                };
                await _orderItemRepository.AddAsync(orderItem);
                order.OrderItems.Add(orderItem);
            }

            await _notificationService.SendOrderNotificationAsync(customer.StoreId, roomId);
            return new ApiResponse<Order>(order, "تم إضافة الطلب بنجاح", 201);
        }

        public async Task<ApiResponse<List<Order>>> GetPendingOrdersAsync(int storeId)
        {
            var orders = await _orderRepository.FindWithIncludesAsync(
                predicate: o => o.Customer.StoreId == storeId && o.Status == 0,
                include: q => q.Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem),
                includeDeleted: false);

            return new ApiResponse<List<Order>>(orders.ToList());
        }

        public async Task<ApiResponse<List<Order>>> GetOrdersAsync(int storeId, bool includeDeleted = false)
        {
            var orders = await _orderRepository.FindWithIncludesAsync(
                predicate: o => o.Customer.StoreId == storeId,
                include: q => q.Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem),
                includeDeleted: includeDeleted);

            return new ApiResponse<List<Order>>(orders.ToList());
        }

        public async Task<ApiResponse<Order>> ApproveOrderAsync(int orderId, decimal totalAmount)
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(orderId, include: q => q.Include(o => o.Customer).Include(o => o.Room));
            if (order == null)
            {
                return new ApiResponse<Order>("الطلب غير موجود", 404);
            }

            order.Status = OrderStatus.Accepted;
            order.TotalAmount = totalAmount;
            order.LastModifiedOn = DateTime.UtcNow;
            _orderRepository.Update(order);

            var pointSetting = await _pointSettingRepository.GetByIdWithIncludesAsync(0, include: q => q.Where(ps => ps.StoreId == order.Customer.StoreId));
            if (pointSetting != null)
            {
                var points = (int)(totalAmount / pointSetting.Amount * pointSetting.Points);
                order.Customer.Points += points;
                _customerRepository.Update(order.Customer);
            }

            await _notificationService.SendOrderStatusUpdateAsync(order.RoomId, true);
            return new ApiResponse<Order>(order, "تم الموافقة على الطلب بنجاح");
        }

        public async Task<ApiResponse<Order>> RejectOrderAsync(int orderId, string rejectionReason)
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(orderId, include: q => q.Include(o => o.Room));
            if (order == null)
            {
                return new ApiResponse<Order>("الطلب غير موجود", 404);
            }

            order.Status = OrderStatus.Rejected;
            order.RejectionReason = rejectionReason;
            order.LastModifiedOn = DateTime.UtcNow;
            _orderRepository.Update(order);

            await _notificationService.SendOrderStatusUpdateAsync(order.RoomId, false, rejectionReason);
            return new ApiResponse<Order>(order, "تم رفض الطلب بنجاح");
        }

        public async Task<ApiResponse<int>> GetTotalOrdersCountAsync(int storeId)
        {
            var count = (await _orderRepository.FindAsync(o => o.Customer.StoreId == storeId)).Count();
            return new ApiResponse<int>(count, "تم جلب عدد الطلبات بنجاح");
        }
    }
}