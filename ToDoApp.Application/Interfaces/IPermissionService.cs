using System.Security.Claims;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services;

public interface IPermissionService
{
    bool HasPermission(ClaimsPrincipal user, UserPermission permission);
}