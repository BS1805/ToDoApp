using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Services;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ToDoService _toDoService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        ToDoService toDoService,
        ILogger<AdminController> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Error");
            }

            _logger.LogInformation($"User with ID {id} was deleted.");
            return RedirectToAction("ViewAllUsers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deleting the user with ID {id}.");
            return View("Error");
        }
    }

    public async Task<IActionResult> ViewAllUsers()
    {
        var users = _userManager.Users.ToList();

        var userList = new List<dynamic>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var taskCount = _toDoService.GetAllToDoItems().Result.Count(item => item.UserId == user.Id);

            userList.Add(new
            {
                User = user,
                Roles = roles,
                TaskCount = taskCount
            });
        }

        return View(userList);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePermissions([FromBody] UpdatePermissionsRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.UserId))
            return BadRequest("Invalid request data.");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound("User not found.");

        user.Permissions = request.Permissions;
        var result = await _userManager.UpdateAsync(user);

     
        var claims = await _userManager.GetClaimsAsync(user);
        var permissionClaim = claims.FirstOrDefault(c => c.Type == "Permissions");
        if (permissionClaim != null)
            await _userManager.RemoveClaimAsync(user, permissionClaim);
        await _userManager.AddClaimAsync(user, new Claim("Permissions", user.Permissions.ToString()));

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { success = true });
    }

    public class UpdatePermissionsRequest
    {
        public string UserId { get; set; }
        public UserPermission Permissions { get; set; }
    }



}