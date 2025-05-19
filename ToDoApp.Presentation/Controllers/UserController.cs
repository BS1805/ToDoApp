using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Presentation.Controllers;

public class UserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var client = _httpClientFactory.CreateClient("ToDoApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/todo/user?page={page}&pageSize={pageSize}");
        if (response.IsSuccessStatusCode)
        {
            var tasks = await response.Content.ReadFromJsonAsync<PagedListViewModel<TaskViewModel>>();
            return View(tasks);
        }

        ModelState.AddModelError(string.Empty, "Failed to load tasks.");
        return View("Error");
    }
    // GET: /User/Create
    public IActionResult Create() => View(new TaskViewModel());

    // POST: /User/Create
    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await _httpClient.PostAsJsonAsync($"/api/todo?userId={userId}", model);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to create task.");
        return View(model);
    }

    // GET: /User/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var task = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/todo/{id}?userId={userId}");
            return View(task);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Failed to load task.");
            return View("Error");
        }
    }

    // POST: /User/Edit
    [HttpPost]
    public async Task<IActionResult> Edit(TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await _httpClient.PutAsJsonAsync($"/api/todo/{model.Id}?userId={userId}", model);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to update task.");
        return View(model);
    }

    // GET: /User/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var task = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/todo/{id}?userId={userId}");
            return View(task);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Failed to load task.");
            return View("Error");
        }
    }

    // POST: /User/Delete/{id}
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await _httpClient.DeleteAsync($"/api/todo/{id}?userId={userId}");
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to delete task.");
        return View("Error");
    }

    // GET: /User/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var task = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/todo/{id}?userId={userId}");
            return View(task);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Failed to load task details.");
            return View("Error");
        }
    }
}
