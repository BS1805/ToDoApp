using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using ToDoApp.FrontEnd.Models;

public class ToDoController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ToDoController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:44369/api/todo/tasks/dashboard");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var dashboardData = await response.Content.ReadFromJsonAsync<List<DashboardTaskSummaryDto>>();
        return View(dashboardData);
    }

    [HttpGet]
    public async Task<IActionResult> TasksByStatus(int statusId, int page = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://localhost:44369/api/todo/tasks/status/{statusId}?page={page}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var pagedTasks = await response.Content.ReadFromJsonAsync<PagedListViewModel<TaskViewModel>>();
        ViewData["StatusId"] = statusId;
        ViewData["PageSize"] = pageSize;
        return View(pagedTasks);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://localhost:44369/api/todo/user?page={page}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var pagedTasks = await response.Content.ReadFromJsonAsync<PagedListViewModel<TaskViewModel>>();
        ViewData["PageSize"] = pageSize;
        return View(pagedTasks);
    }



    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://localhost:44369/api/todo/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var task = await response.Content.ReadFromJsonAsync<TaskViewModel>();
        return View(task);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:44369/api/todo/statuses");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var statuses = await response.Content.ReadFromJsonAsync<List<Status>>();
        ViewBag.Statuses = statuses;

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient();
        var content = JsonContent.Create(model);
        var response = await client.PostAsync($"https://localhost:44369/api/todo", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to create task.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://localhost:44369/api/todo/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var task = await response.Content.ReadFromJsonAsync<TaskViewModel>();

        var statusesResponse = await client.GetAsync("https://localhost:44369/api/todo/statuses");
        if (!statusesResponse.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var statuses = await statusesResponse.Content.ReadFromJsonAsync<List<Status>>();
        ViewBag.Statuses = statuses;

        return View(task);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(int id, TaskViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient();
        var content = JsonContent.Create(model);
        var response = await client.PutAsync($"https://localhost:44369/api/todo/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to update task.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.DeleteAsync($"https://localhost:44369/api/todo/{id}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to delete task.");
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }
}
