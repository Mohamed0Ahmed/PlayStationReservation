using System.Domain.Models;
using System.Application.Abstraction;
using System.Shared.Exceptions;
using System.Infrastructure.Unit;

namespace System.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            var customer = await _unitOfWork.GetRepository<Customer, int>().GetByIdAsync(id);
            if (customer == null)
                throw new CustomException("Customer not found.", 404);
            return customer;
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phoneNumber , int storeId)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new CustomException("Phone number is required.", 400);

            var customer = (await _unitOfWork.GetRepository<Customer, int>().FindAsync(c => c.PhoneNumber == phoneNumber && !c.IsDeleted)).Where(c=>c.StoreId == storeId).FirstOrDefault();
            return customer!;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<Customer, int>().GetAllAsync(includeDeleted);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
                throw new CustomException("Phone number is required.", 400);
            if (customer.Points < 0)
                throw new CustomException("Points cannot be negative.", 400);

            var existingCustomer = await GetCustomerByPhoneAsync(customer.PhoneNumber , customer.StoreId);
            if (existingCustomer != null)
                throw new CustomException("Customer with this phone number already exists.", 400);

            await _unitOfWork.GetRepository<Customer, int>().AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await GetCustomerByIdAsync(customer.Id);
            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
                throw new CustomException("Phone number is required.", 400);
            if (customer.Points < 0)
                throw new CustomException("Points cannot be negative.", 400);

            var duplicateCustomer = await GetCustomerByPhoneAsync(customer.PhoneNumber , customer.StoreId);
            if (duplicateCustomer != null && duplicateCustomer.Id != customer.Id)
                throw new CustomException("Another customer with this phone number already exists.", 400);

            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Points = customer.Points;

            _unitOfWork.GetRepository<Customer, int>().Update(existingCustomer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await GetCustomerByIdAsync(id);
            _unitOfWork.GetRepository<Customer, int>().Delete(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreCustomerAsync(int id)
        {
            var customer = await _unitOfWork.GetRepository<Customer, int>().GetByIdAsync(id, true);
            if (customer == null)
                throw new CustomException("Customer not found.", 404);
            if (!customer.IsDeleted)
                throw new CustomException("Customer is not deleted.", 400);

            await _unitOfWork.GetRepository<Customer, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}