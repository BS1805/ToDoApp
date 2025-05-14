using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Presentation.Controllers;

[Authorize(Policy = "UserOnly")]
public class UserController : Controller
{
    private readonly IToDoService _toDoService;

    public UserController(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var model = await _toDoService.GetPagedToDoItemsAsync(userId, page, pageSize);
        return View(model);
    }

    public IActionResult Create()
    {
        return View(new TaskViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.Id = int.Parse(userId); // Ensure this maps correctly
            await _toDoService.AddToDoItemAsync(model);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _toDoService.GetToDoItemByIdAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _toDoService.UpdateToDoItemAsync(model);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _toDoService.GetToDoItemByIdAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _toDoService.DeleteToDoItemAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
