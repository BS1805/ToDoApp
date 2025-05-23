using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Enums;

namespace ToDoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUserAdminService _userAdminService;

    public AdminController(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> Index()
    {
        var userDtos = await _userAdminService.GetAllUsersWithDetailsAsync();
        return Ok(userDtos);
    }

    [HttpPost("users/{userId}/activate")]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        var success = await _userAdminService.ActivateUserAsync(userId);
        if (!success) return BadRequest("Failed to activate user.");
        return NoContent();
    }

    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var success = await _userAdminService.DeactivateUserAsync(userId);
        if (!success) return BadRequest("Failed to deactivate user.");
        return NoContent();
    }

    [HttpPut("permissions")]
    public async Task<IActionResult> UpdatePermissions([FromBody] UpdatePermissionsRequest request)
    {
        var combinedPermissions = request.Permissions?.Aggregate(0, (current, permission) => current | permission) ?? 0;
        var success = await _userAdminService.UpdateUserPermissionsAsync(request.UserId, (UserPermission)combinedPermissions);
        if (!success)
            return BadRequest("Failed to update permissions.");
        return NoContent();
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var success = await _userAdminService.DeleteUserAsync(userId);
        if (!success)
            return BadRequest("Failed to delete user.");
        return NoContent();
    }
}