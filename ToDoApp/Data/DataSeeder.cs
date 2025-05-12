using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using ToDoApp.Models;

namespace ToDoApp.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            // Role Manager
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // User Manager
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "hasib@gmail.com";
            var adminPassword = "123";

            // Check if the admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
