using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Persistence;

public class DataSeeder
{
    public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminEmail = "hasib@gmail.com";
        var adminPassword = "123";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task SeedStatuses(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

       
        await dbContext.Database.EnsureCreatedAsync();

      
        if (!await dbContext.Statuses.AnyAsync())
        {
            var statuses = new List<Status>
   {
       new Status { Name = "Pending" },
       new Status { Name = "In Progress" },
       new Status { Name = "Completed" }
   };


            await dbContext.Statuses.AddRangeAsync(statuses);
            await dbContext.SaveChangesAsync();
        }
    }

    public static async Task SeedAll(IServiceProvider serviceProvider)
    {
        await SeedRolesAndAdminUser(serviceProvider);
        await SeedStatuses(serviceProvider);
    }
}

