using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Repositories;
using System.Shared;

namespace System.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer, int> _customerRepository;

        public CustomerService(IRepository<Customer, int> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ApiResponse<Customer>> RegisterCustomerAsync(string phoneNumber, int storeId)
        {
            var existingCustomer = await _customerRepository.GetByIdWithIncludesAsync(
                id: 0,
                include: q => q.Where(c => c.PhoneNumber == phoneNumber && c.StoreId == storeId),
                includeDeleted: false);

            if (existingCustomer != null)
            {
                return new ApiResponse<Customer>("رقم التليفون موجود بالفعل", 400);
            }

            var customer = new Customer
            {
                PhoneNumber = phoneNumber,
                StoreId = storeId,
                Points = 0,
                CreatedOn = DateTime.UtcNow
            };

            await _customerRepository.AddAsync(customer);
            return new ApiResponse<Customer>(customer, "تم التسجيل بنجاح", 201);
        }

        public async Task<ApiResponse<Customer>> GetCustomerByPhoneAsync(string phoneNumber, int storeId)
        {
            var customer = await _customerRepository.GetByIdWithIncludesAsync(
                id: 0,
                include: q => q.Where(c => c.PhoneNumber == phoneNumber && c.StoreId == storeId),
                includeDeleted: false);

            if (customer == null)
            {
                return new ApiResponse<Customer>(" غير موجود", 404);
            }

            return new ApiResponse<Customer>(customer);
        }

        public async Task<ApiResponse<int>> GetCustomerPointsAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiResponse<int>(" غير موجود", 404);
            }

            return new ApiResponse<int>(customer.Points);
        }
    }
}