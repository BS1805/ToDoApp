namespace ToDoApp.Domain.Enums;

[Flags]
public enum UserPermission
{
    None = 0,
    Create = 1,
    Details = 2,
    Edit = 4,
    Delete = 8
}
