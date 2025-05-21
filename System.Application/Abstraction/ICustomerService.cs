using System.Domain.Models;
using System.Shared;
using System.Shared.DTOs.Customers;

namespace System.Application.Abstraction
{
    public interface ICustomerService
    {
        Task<ApiResponse<Customer>> LoginCustomer(string phoneNumber, int storeId);
        Task<ApiResponse<CustomerDto>> GetCustomerByPhoneAsync(string phoneNumber, int storeId);
        Task<ApiResponse<Customer>> UpdateCustomerAsync(int customerId, string phoneNumber, int storeId);
        Task<ApiResponse<bool>> DeleteCustomerAsync(int customerId);
        Task<ApiResponse<bool>> RestoreCustomerAsync(int customerId);
    }
}