using Microsoft.AspNetCore.SignalR;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;
using System.Shared;
using System.Domain.Enums;

namespace System.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly IRoomService _roomService;
        private readonly IPointSettingService _pointSettingService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderService(
            IUnitOfWork unitOfWork,
            ICustomerService customerService,
            IRoomService roomService,
            IPointSettingService pointSettingService,
            IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _roomService = roomService;
            _pointSettingService = pointSettingService;
            _hubContext = hubContext;
        }

        private async Task<int> CalculatePointsEarned(Order order)
        {
            var room = await _roomService.GetRoomByIdAsync(order.RoomId);
            var pointSettings = await _pointSettingService.GetPointSettingsByStoreAsync(room.StoreId);
            var pointSetting = pointSettings.OrderByDescending(ps => ps.Amount).FirstOrDefault(ps => ps.Amount <= order.TotalAmount);
            return pointSetting?.Points ?? 0;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithIncludesAsync(id, false, o => o.OrderItems);
            if (order == null)
                throw new CustomException("Order not found.", 404);
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByRoomAsync(int roomId, bool includeDeleted = false)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            return await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(o => o.RoomId == roomId, includeDeleted, o => o.OrderItems, o => o.Customer, o => o.Room);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStoreAsync(int storeId, bool includeDeleted = false)
        {
            var rooms = await _unitOfWork.GetRepository<Room, int>().FindAsync(r => r.StoreId == storeId, includeDeleted);
            var roomIds = rooms.Select(r => r.Id).ToList();
            return await _unitOfWork.GetRepository<Order, int>().FindWithIncludesAsync(
                o => roomIds.Contains(o.RoomId),
                includeDeleted,
                o => o.OrderItems,
                o => o.Customer,
                o => o.Room
            );
        }

        public async Task AddOrderAsync(Order order)
        {
            if (order.TotalAmount < 0)
                throw new CustomException("Total amount cannot be negative.", 400);
            if (string.IsNullOrWhiteSpace(order.PaymentMethod))
                throw new CustomException("Payment method is required.", 400);
            if (order.PointsUsed < 0)
                throw new CustomException("Points used cannot be negative.", 400);

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (order.PointsUsed > customer.Points)
                throw new CustomException("Customer does not have enough points.", 400);

            await _roomService.GetRoomByIdAsync(order.RoomId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Points are not calculated here; they will be calculated when the order is accepted

                await _unitOfWork.CommitTransactionAsync();

                // Send notification to the store owner
                await _hubContext.Clients.Group($"Store_{customer.StoreId}")
                    .SendAsync("ReceiveOrderNotification", $"New order placed (ID: {order.Id})");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to add order.", 500);
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = await GetOrderByIdAsync(order.Id);
            if (order.TotalAmount < 0)
                throw new CustomException("Total amount cannot be negative.", 400);
            if (string.IsNullOrWhiteSpace(order.PaymentMethod))
                throw new CustomException("Payment method is required.", 400);
            if (order.PointsUsed < 0)
                throw new CustomException("Points used cannot be negative.", 400);

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var tempPoints = customer.Points + existingOrder.PointsUsed;
            if (order.PointsUsed > tempPoints)
                throw new CustomException("Customer does not have enough points after reverting previous points.", 400);

            await _roomService.GetRoomByIdAsync(order.RoomId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Revert previous points adjustments
                customer.Points += existingOrder.PointsUsed;

                // Handle points based on the new status
                if (order.Status == OrderStatus.Accepted && existingOrder.Status != OrderStatus.Accepted)
                {
                    // Order is being accepted
                    if (order.PaymentMethod == "Cash")
                    {
                        var pointsEarned = await CalculatePointsEarned(order);
                        customer.Points += pointsEarned;
                    }
                    customer.Points -= order.PointsUsed;
                }
                else if (order.Status == OrderStatus.Rejected && existingOrder.Status != OrderStatus.Rejected)
                {
                    // Order is being rejected; revert points used
                    customer.Points -= order.PointsUsed;
                }

                //await _customerService.UpdateCustomerAsync(customer);

                existingOrder.CustomerId = order.CustomerId;
                existingOrder.RoomId = order.RoomId;
                existingOrder.TotalAmount = order.TotalAmount;
                existingOrder.PaymentMethod = order.PaymentMethod;
                existingOrder.PointsUsed = order.PointsUsed;
                existingOrder.Status = order.Status;
                existingOrder.RejectionReason = order.RejectionReason ?? "Accepted";
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.GetRepository<Order, int>().Update(existingOrder);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to update order.", 500);
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await GetOrderByIdAsync(id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Soft delete all OrderItems
                if (order.OrderItems != null)
                {
                    foreach (var orderItem in order.OrderItems.Where(oi => !oi.IsDeleted))
                    {
                        _unitOfWork.GetRepository<OrderItem, int>().Delete(orderItem);
                    }
                }

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                customer.Points += order.PointsUsed; // Revert points used
                await _customerService.UpdateCustomerAsync(customer);

                _unitOfWork.GetRepository<Order, int>().Delete(order);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to delete order.", 500);
            }
        }

        public async Task RestoreOrderAsync(int id)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithIncludesAsync(id, true, o => o.OrderItems);
            if (order == null)
                throw new CustomException("Order not found.", 404);
            if (!order.IsDeleted)
                throw new CustomException("Order is not deleted.", 400);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var pointsEarned = await CalculatePointsEarned(order);
                if (order.PointsUsed > customer.Points)
                    throw new CustomException("Customer does not have enough points to restore this order.", 400);

                customer.Points += pointsEarned;
                customer.Points -= order.PointsUsed;
                await _customerService.UpdateCustomerAsync(customer);

                // Restore all OrderItems
                if (order.OrderItems != null)
                {
                    foreach (var orderItem in order.OrderItems.Where(oi => oi.IsDeleted))
                    {
                        await _unitOfWork.GetRepository<OrderItem, int>().RestoreAsync(orderItem.Id);
                    }
                }

                await _unitOfWork.GetRepository<Order, int>().RestoreAsync(id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new CustomException("Failed to restore order.", 500);
            }
        }
    }
}