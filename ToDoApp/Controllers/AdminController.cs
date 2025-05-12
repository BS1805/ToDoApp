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

    public IActionResult Index()
    {
        try
        {
            var users = _userManager.Users;
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the Admin Index page.");
            return View("Error");
        }
    }

    public async Task<IActionResult> ViewUserToDos(string userId)
    {
        try
        {
            var toDoItems = await _toDoService.GetAllToDoItems();
            return View(toDoItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving ToDo items for user {userId}.");
            return View("Error");
        }
    }
}
