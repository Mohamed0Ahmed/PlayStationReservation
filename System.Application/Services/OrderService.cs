using System.Domain.Models;
using System.Application.Abstraction;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdAsync(id);
            if (order == null)
                throw new Exception("Order not found.");
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByRoomAsync(int roomId, bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<Order, int>().FindAsync(o => o.RoomId == roomId, includeDeleted);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = await GetOrderByIdAsync(order.Id);
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
            _unitOfWork.GetRepository<Order, int>().Delete(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreOrderAsync(int id)
        {
            await _unitOfWork.GetRepository<Order, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}