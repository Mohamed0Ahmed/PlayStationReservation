using Microsoft.AspNetCore.Identity;
using System.Infrastructure;
using System.Application;
using System.Shared.Middleware;
using System.Infrastructure.Data;

namespace MvcProject
{
    public class Program
    {
        public static async Task Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);

            #region services
            builder.Services.AddControllersWithViews();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();
            #endregion

            var app = builder.Build();

            #region Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseExceptionHandlerMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            #endregion

            // Seed an admin user
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                await SeedAdminUser(userManager);
            }

            await app.RunAsync(); 
        }

        private static async Task SeedAdminUser(UserManager<IdentityUser> userManager)
        {
            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, adminPassword);
            }
        }
    }
}