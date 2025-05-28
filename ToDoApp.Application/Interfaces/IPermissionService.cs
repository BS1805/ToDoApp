using System.Security.Claims;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services;

public interface IPermissionService
{
    object HasPermission(ClaimsPrincipal user, UserPermission permission);
}
