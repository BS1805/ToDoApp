using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces;

public interface IUserAdminService
{
    Task<object> GetAllUsersWithRolesAndTaskCountAsync();
    Task<object> UpdateUserPermissionsAsync(string userId, UserPermission permissions);
    Task<object> GetAllUsersWithDetailsAsync();
    Task<object> DeleteUserAsync(string userId);
    Task<object> ActivateUserAsync(string userId);
    Task<object> DeactivateUserAsync(string userId);
}
