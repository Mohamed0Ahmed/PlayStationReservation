using Mapster;
using System.Application.Abstraction;
using System.Domain.Models;
using System.Infrastructure.Unit;
using System.Shared;
using System.Shared.DTOs.Customers;

namespace System.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Customers

        //* Login Customer
        public async Task<ApiResponse<Customer>> LoginCustomer(string phoneNumber, int storeId)
        {
            var existingCustomer = await _unitOfWork.GetRepository<Customer, int>().FindAsync(c => c.PhoneNumber == phoneNumber && c.StoreId == storeId);

            var customer = new Customer
            {
                PhoneNumber = phoneNumber,
                StoreId = storeId,
                Points = 0,
                CreatedOn = DateTime.UtcNow
            };

            if (!existingCustomer.Any())
            {
                await _unitOfWork.GetRepository<Customer, int>().AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<Customer>(customer, "تم اضافتك بنجاح", 201);
            }

            return new ApiResponse<Customer>(existingCustomer.FirstOrDefault()!, "تم التسجيل بنجاح", 201);
        }


        //* Get Customer by Phone
        public async Task<ApiResponse<CustomerDto>> GetCustomerByPhoneAsync(string phoneNumber, int storeId)
        {
            var customers = await _unitOfWork.GetRepository<Customer, int>().FindWithIncludesAsync(
                c => c.PhoneNumber == phoneNumber && c.StoreId == storeId);
            var customer = customers.FirstOrDefault();
            if (customer == null)
                return new ApiResponse<CustomerDto>("العميل غير موجود", 200);

            var customerDto = customer.Adapt<CustomerDto>();

            return new ApiResponse<CustomerDto>(customerDto, "تم جلب بيانات العميل بنجاح");
        }




        //* Update Customer
        public async Task<ApiResponse<Customer>> UpdateCustomerAsync(int customerId, string phoneNumber, int storeId)
        {
            if (customerId <= 0 || string.IsNullOrEmpty(phoneNumber) || storeId <= 0)
            {
                return new ApiResponse<Customer>("ادخل البيانات بشكل صحيح", 200);
            }

            var customer = await _unitOfWork.GetRepository<Customer, int>().GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiResponse<Customer>("العميل غير موجود", 200);
            }

            var existingCustomer = await _unitOfWork.GetRepository<Customer, int>().FindWithIncludesAsync(
                c => c.PhoneNumber == phoneNumber && c.StoreId == storeId && c.Id != customerId,
                includeDeleted: false);
            if (existingCustomer != null)
            {
                return new ApiResponse<Customer>("رقم التليفون موجود بالفعل لعميل آخر", 200);
            }

            customer.PhoneNumber = phoneNumber;
            customer.StoreId = storeId;
            customer.LastModifiedOn = DateTime.UtcNow;

            _unitOfWork.GetRepository<Customer, int>().Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<Customer>(customer, "تم تعديل بيانات العميل بنجاح");
        }


        //* Delete Customer
        public async Task<ApiResponse<bool>> DeleteCustomerAsync(int customerId)
        {
            if (customerId <= 0)
            {
                return new ApiResponse<bool>("الرقم ده مش موجود", 200);
            }

            var customer = await _unitOfWork.GetRepository<Customer, int>().GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiResponse<bool>("الرقم ده مش موجود", 200);
            }

            _unitOfWork.GetRepository<Customer, int>().Delete(customer);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم حذف العميل بنجاح");
        }


        //* Restore Customer
        public async Task<ApiResponse<bool>> RestoreCustomerAsync(int customerId)
        {
            if (customerId <= 0)
            {
                return new ApiResponse<bool>("الرقم ده مش موجود", 200);
            }

            var customer = await _unitOfWork.GetRepository<Customer, int>().GetByIdAsync(customerId, onlyDeleted: true);
            if (customer == null || !customer.IsDeleted)
            {
                return new ApiResponse<bool>("العميل غير موجود أو غير محذوف", 404);
            }

            await _unitOfWork.GetRepository<Customer, int>().RestoreAsync(customer.Id);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "تم استرجاع العميل بنجاح");
        }

        #endregion
    }
}