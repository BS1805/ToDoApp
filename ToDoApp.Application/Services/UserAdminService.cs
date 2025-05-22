using Microsoft.AspNetCore.Identity;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using System.Security.Claims;
using ToDoApp.Application.DTOs;


namespace ToDoApp.Application.Services;

public class UserAdminService : IUserAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;

    private readonly IToDoService _toDoService;

    public UserAdminService(UserManager<ApplicationUser> userManager, IToDoService toDoService, IUserRepository userRepository)
    {
        _userManager = userManager;
        _toDoService = toDoService;
        _userRepository = userRepository;
    }

    public async Task<bool> ActivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.LockoutEnabled = false;
        user.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _userRepository.TransferTasksToActiveAsync(userId);
        }

        return result.Succeeded;
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _userRepository.TransferTasksToArchiveAsync(userId);
        }

        return result.Succeeded;
    }
    public async Task<List<(ApplicationUser User, IList<string> Roles, int TaskCount)>> GetAllUsersWithRolesAndTaskCountAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<(ApplicationUser, IList<string>, int)>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            int taskCount = await _toDoService.GetTaskCountForUserAsync(user.Id); 
            result.Add((user, roles, taskCount));
        }
        return result;
    }


    public async Task<List<AdminUserDto>> GetAllUsersWithDetailsAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<AdminUserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            int taskCount = await _toDoService.GetTaskCountForUserAsync(user.Id);
            var permissions = user.Permissions != null
                ? ((UserPermission)user.Permissions).ToString()
                    .Split(", ")
                    .Select(p => (int)Enum.Parse(typeof(UserPermission), p))
                    .ToList()
                : new List<int>();

            result.Add(new AdminUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = roles,
                TaskCount = taskCount,
                Permissions = permissions
            });
        }
        return result;
    }

    public async Task<bool> UpdateUserPermissionsAsync(string userId, UserPermission permissions)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.Permissions = permissions;
        var updateResult = await _userManager.UpdateAsync(user);

        // Update Permissions claim
        var claims = await _userManager.GetClaimsAsync(user);
        var permissionClaim = claims.FirstOrDefault(c => c.Type == "Permissions");
        if (permissionClaim != null)
            await _userManager.RemoveClaimAsync(user, permissionClaim);
        await _userManager.AddClaimAsync(user, new Claim("Permissions", ((int)user.Permissions).ToString()));

        return updateResult.Succeeded;
    }



    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
