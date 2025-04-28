using Microsoft.Extensions.DependencyInjection;
using System.Application.Abstraction;
using System.Application.Services;

namespace System.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IAssistanceRequestService, AssistanceRequestService>();
            services.AddScoped<IPointSettingService, PointSettingService>();


            return services;
        }
    }
}