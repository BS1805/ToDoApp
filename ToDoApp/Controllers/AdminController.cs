using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers;

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



}
