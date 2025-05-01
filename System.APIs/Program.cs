using System.Infrastructure;
using System.Application;
using System.Shared;
using System.Infrastructure.Repositories;
using System.Infrastructure.Unit;

namespace System.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services

            builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            builder.Services.AddAuthorization();

            builder.Services.AddInfrastructure(builder.Configuration); 
            builder.Services.AddApplication();
            builder.Services.AddSignalR();


            #endregion


            var app = builder.Build();





            #region Middleware

            app.MapHub<NotificationHub>("/notificationHub");

            app.UseHttpsRedirection();
            app.UseAuthorization();

            #endregion


          

        

            app.Run();
        }
    }
}
