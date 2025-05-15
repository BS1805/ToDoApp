using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly IUserAdminService _userAdminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IUserAdminService userAdminService, ILogger<AdminController> logger)
    {
        _userAdminService = userAdminService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePermissions([FromBody] UpdatePermissionsRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.UserId))
            return BadRequest("Invalid request data.");

        var success = await _userAdminService.UpdateUserPermissionsAsync(request.UserId, request.Permissions);
        if (!success)
            return BadRequest("Failed to update permissions.");

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var success = await _userAdminService.DeleteUserAsync(id);
        if (!success)
            return BadRequest("Failed to delete user.");
        return RedirectToAction("ViewAllUsers");
    }

    public async Task<IActionResult> ViewAllUsers()
    {
        var usersWithRoles = await _userAdminService.GetAllUsersWithRolesAndTaskCountAsync();
        var userList = usersWithRoles.Select(ur => new
        {
            User = ur.User,
            Roles = ur.Roles,
            TaskCount = ur.TaskCount
        }).ToList();
        return View(userList);
    }

}
