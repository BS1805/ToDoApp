using System.Security.Claims;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services;

public class PermissionService : IPermissionService
{
    public bool HasPermission(ClaimsPrincipal user, UserPermission permission)
    {
        var permissionsClaim = user.FindFirst("Permissions")?.Value;
        if (string.IsNullOrEmpty(permissionsClaim)) return false;

        if (int.TryParse(permissionsClaim, out var userPermissions))
        {
            return (userPermissions & (int)permission) == (int)permission;
        }

        return false;
    }
}
