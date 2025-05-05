using Microsoft.Extensions.DependencyInjection;
using System.Application.Abstraction;
using System.Application.Services;


namespace System.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            // Register services
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IGiftService, GiftService>();
            services.AddScoped<IAssistanceRequestTypeService, AssistanceRequestTypeService>();

            return services;
        }
    }
}