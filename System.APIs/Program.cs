using Microsoft.AspNetCore.Identity;
using System.Infrastructure;
using System.Application;
using System.Shared;
using System.Infrastructure.Repositories;
using System.Infrastructure.Unit;
using System.Infrastructure.Data;

namespace System.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services

            builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddControllers(); 

            // Add Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthorization();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddSignalR();

            // Add Swagger
            builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

            #endregion

            var app = builder.Build();

            #region Middleware

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapHub<NotificationHub>("/notificationHub");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers(); 

            #endregion

            #region Seed Data

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Seed Roles
                string[] roles = { "Admin", "Owner" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Seed Default Admin
                var adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@system.com"
                };
                string adminPassword = "Admin@123";

                var user = await userManager.FindByNameAsync(adminUser.UserName);
                if (user == null)
                {
                    var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createAdmin.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }

            #endregion

            app.Run();
        }
    }
}