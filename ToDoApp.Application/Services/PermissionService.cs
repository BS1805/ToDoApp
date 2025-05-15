using System.Security.Claims;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services;

public class PermissionService
{
    public bool HasPermission(ClaimsPrincipal user, UserPermission requiredPermission)
    {
        var userPermissions = (UserPermission)Enum.Parse(typeof(UserPermission), user.FindFirstValue("Permissions") ?? "None");
        return userPermissions.HasFlag(requiredPermission);
    }
}
