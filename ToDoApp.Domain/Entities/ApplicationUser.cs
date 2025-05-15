using Microsoft.AspNetCore.Identity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public UserPermission Permissions { get; set; } = UserPermission.None;
}
