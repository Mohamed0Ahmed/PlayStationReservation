using System.Domain.Models;
using System.Application.Abstraction;
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
                throw new Exception("Customer not found.");
            return customer;
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phoneNumber)
        {
            var customer = (await _unitOfWork.GetRepository<Customer, int>().FindAsync(c => c.PhoneNumber == phoneNumber))
                .FirstOrDefault();
            return customer!;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeDeleted = false)
        {
            return await _unitOfWork.GetRepository<Customer, int>().GetAllAsync(includeDeleted);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _unitOfWork.GetRepository<Customer, int>().AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await GetCustomerByIdAsync(customer.Id);
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
            await _unitOfWork.GetRepository<Customer, int>().RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}