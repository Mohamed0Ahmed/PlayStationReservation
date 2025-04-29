using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly IRoomService _roomService;
        private readonly IOrderItemService _orderItemService;
        private readonly IPointSettingService _pointSettingService;

        public OrderService(IUnitOfWork unitOfWork, ICustomerService customerService, IRoomService roomService, IOrderItemService orderItemService, IPointSettingService pointSettingService)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _roomService = roomService;
            _orderItemService = orderItemService;
            _pointSettingService = pointSettingService;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(id);
            if (order == null)
                throw new CustomException("Order not found.", 404);
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByRoomAsync(int roomId, bool includeDeleted = false)
        {
            await _roomService.GetRoomByIdAsync(roomId); 
            return await _unitOfWork.GetRepository<Order, int>().FindAsync(o => o.RoomId == roomId, includeDeleted);
        }

        public async Task AddOrderAsync(Order order)
        {
            if (order.TotalAmount < 0)
                throw new CustomException("Total amount cannot be negative.", 400);
            if (string.IsNullOrWhiteSpace(order.PaymentMethod))
                throw new CustomException("Payment method is required.", 400);
            if (order.PointsUsed < 0)
                throw new CustomException("Points used cannot be negative.", 400);

            await _customerService.GetCustomerByIdAsync(order.CustomerId); 
            await _roomService.GetRoomByIdAsync(order.RoomId); 

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Calculate Points
                var room = await _roomService.GetRoomByIdAsync(order.RoomId);
                var pointSettings = await _pointSettingService.GetPointSettingsByStoreAsync(room.StoreId);
                var pointSetting = pointSettings.OrderByDescending(ps => ps.Amount).FirstOrDefault(ps => ps.Amount <= order.TotalAmount);
                if (pointSetting != null)
                {
                    var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                    customer.Points += pointSetting.Points;
                    await _customerService.UpdateCustomerAsync(customer);
                }

                await _unitOfWork.CommitTransactionAsync();
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

            await _customerService.GetCustomerByIdAsync(order.CustomerId); 
            await _roomService.GetRoomByIdAsync(order.RoomId); 

            existingOrder.CustomerId = order.CustomerId;
            existingOrder.RoomId = order.RoomId;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.PaymentMethod = order.PaymentMethod;
            existingOrder.PointsUsed = order.PointsUsed;
            existingOrder.Status = order.Status;
            existingOrder.RejectionReason = order.RejectionReason;
            existingOrder.OrderDate = order.OrderDate;
            _unitOfWork.GetRepository<Order, int>().Update(existingOrder);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await GetOrderByIdAsync(id);
            var orderItems = await _orderItemService.GetOrderItemsByOrderAsync(id);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var orderItem in orderItems)
                {
                    await _orderItemService.DeleteOrderItemAsync(orderItem.OrderId, orderItem.MenuItemId);
                }

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
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(id, true);
            if (order == null)
                throw new CustomException("Order not found.", 404);
            if (!order.IsDeleted)
                throw new CustomException("Order is not deleted.", 400);

            await _unitOfWork.GetRepository<Order, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}