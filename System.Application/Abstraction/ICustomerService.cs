using System.Domain.Models;
using System.Shared;

namespace System.Application.Abstraction
{
    public interface ICustomerService
    {
        Task<ApiResponse<Customer>> RegisterCustomerAsync(string phoneNumber, int storeId);
        Task<ApiResponse<Customer>> GetCustomerByPhoneAsync(string phoneNumber, int storeId);
        Task<ApiResponse<int>> GetCustomerPointsAsync(int customerId);
    }
}