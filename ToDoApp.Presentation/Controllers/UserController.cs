using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Services;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Enums;
using System.Diagnostics;

namespace ToDoApp.Presentation.Controllers;

[Authorize(Policy = "UserOnly")]
public class UserController : Controller
{
    private readonly IToDoService _toDoService;
    private readonly PermissionService _permissionService;
    private readonly ILogger<UserController> _logger;

    public UserController(IToDoService toDoService, PermissionService permissionService, ILogger<UserController> logger)
    {
        _toDoService = toDoService;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _toDoService.GetPagedToDoItemsAsync(userId, page, pageSize);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the User Index page.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public IActionResult Create()
    {
        if (!_permissionService.HasPermission(User, UserPermission.Create))
            return Forbid();

        return View(new TaskViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!_permissionService.HasPermission(User, UserPermission.Create))
            return Forbid();

        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _toDoService.CreateToDoItem(model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a ToDo item.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the ToDo item.");
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!_permissionService.HasPermission(User, UserPermission.Edit))
            return Forbid();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _toDoService.GetTaskViewModelForUser(id, userId);
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving the ToDo item with ID {id} for editing.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    [HttpPost]
    public async Task<IActionResult> Edit(TaskViewModel model)
    {
        if (!_permissionService.HasPermission(User, UserPermission.Edit))
            return Forbid();

        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _toDoService.UpdateToDoItem(model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the ToDo item with ID {model.Id}.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the ToDo item.");
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!_permissionService.HasPermission(User, UserPermission.Delete))
            return Forbid();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _toDoService.GetTaskViewModelForUser(id, userId);
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving the ToDo item with ID {id} for deletion.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!_permissionService.HasPermission(User, UserPermission.Delete))
            return Forbid();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _toDoService.DeleteToDoItemForUser(id, userId);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deleting the ToDo item with ID {id}.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!_permissionService.HasPermission(User, UserPermission.Details))
            return Forbid();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _toDoService.GetTaskViewModelForUser(id, userId);
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving the details of the ToDo item with ID {id}.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
