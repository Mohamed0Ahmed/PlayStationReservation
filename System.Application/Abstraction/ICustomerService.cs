using System.Domain.Models;

namespace System.Application.Abstraction
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> GetCustomerByPhoneAsync(string phoneNumber, int storeId);
        Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeDeleted = false);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task RestoreCustomerAsync(int id);
    }
}