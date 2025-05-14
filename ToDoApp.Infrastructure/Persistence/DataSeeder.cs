using Microsoft.AspNetCore.Identity;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Persistence;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DataSeeder(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        
        await _context.Database.EnsureCreatedAsync();
    
        await SeedRolesAsync();
  
        await SeedUsersAsync();

        await SeedToDoItemsAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.FindByEmailAsync("admin@todoapp.com") == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@todoapp.com",
                Email = "admin@todoapp.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        if (await _userManager.FindByEmailAsync("user@todoapp.com") == null)
        {
            var normalUser = new ApplicationUser
            {
                UserName = "user@todoapp.com",
                Email = "user@todoapp.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(normalUser, "User@123");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }

    private async Task SeedToDoItemsAsync()
    {
        if (!_context.ToDoItems.Any())
        {
            var user = await _userManager.FindByEmailAsync("user@todoapp.com");
            if (user != null)
            {
                var toDoItems = new List<ToDoItem>
                {
                    new ToDoItem { Title = "Task 1", Description = "Description for Task 1", IsCompleted = false, UserId = user.Id },
                    new ToDoItem { Title = "Task 2", Description = "Description for Task 2", IsCompleted = true, UserId = user.Id }
                };

                _context.ToDoItems.AddRange(toDoItems);
                await _context.SaveChangesAsync();
            }
        }
    }
}
