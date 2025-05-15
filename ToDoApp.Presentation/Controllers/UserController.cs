using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;
using System.Diagnostics;
using ToDoApp.Application.Services;

namespace ToDoApp.Presentation.Controllers;

[Authorize(Policy = "UserOnly")]
public class UserController : Controller
{
    private readonly ToDoService _toDoService;
    private readonly ILogger<UserController> _logger;

    public UserController(ToDoService toDoService, ILogger<UserController> logger)
    {
        _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _toDoService.GetPagedToDoItemsAsync(userId, page, pageSize);


            if (page > model.TotalPages && model.TotalPages > 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.TotalPages, pageSize });
            }

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
        try
        {
            return View(new TaskViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the Create page.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var toDoItem = new ToDoItem
                {
                    Title = model.Title,
                    Description = model.Description,
                    IsCompleted = model.IsCompleted,
                    UserId = userId
                };
                await _toDoService.AddToDoItem(toDoItem);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a ToDo item.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the ToDo item.");
            }
        }
        else
        {
            // Log validation errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }
        }
        return View(model);
    }


    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _toDoService.GetToDoItemById(id);

            if (item == null || item.UserId != userId)
            {
                return NotFound();
            }

            var model = new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            };

            return View(model);
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
        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var item = await _toDoService.GetToDoItemById(model.Id.Value);
                if (item == null || item.UserId != userId)
                {
                    return Unauthorized();
                }

                item.Title = model.Title;
                item.Description = model.Description;
                item.IsCompleted = model.IsCompleted;

                await _toDoService.UpdateToDoItem(item);
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
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _toDoService.GetToDoItemById(id);

            if (item == null || item.UserId != userId)
            {
                return NotFound();
            }

            var model = new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving the ToDo item with ID {id} for deletion.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }



    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(TaskViewModel model)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _toDoService.GetToDoItemById(model.Id.Value);

            if (item == null || item.UserId != userId)
            {
                return Unauthorized();
            }

            await _toDoService.DeleteToDoItem(model.Id.Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deleting the ToDo item with ID {model.Id}.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _toDoService.GetToDoItemById(id);

            if (item == null || item.UserId != userId)
            {
                return NotFound();
            }

            var model = new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving the details of the ToDo item with ID {id}.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}