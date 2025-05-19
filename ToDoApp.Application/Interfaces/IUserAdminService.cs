using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces;

public interface IUserAdminService
{
    Task<List<(ApplicationUser User, IList<string> Roles, int TaskCount)>> GetAllUsersWithRolesAndTaskCountAsync();
    Task<bool> UpdateUserPermissionsAsync(string userId, UserPermission permissions);
    Task<List<AdminUserDto>> GetAllUsersWithDetailsAsync();
    Task<bool> DeleteUserAsync(string userId);
}
