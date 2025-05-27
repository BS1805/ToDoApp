using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ToDoApp.FrontEnd.Models;

public class ToDoController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;

    public ToDoController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = apiSettings.Value.BaseUrl.TrimEnd('/');
    }

    private IActionResult HandleUnsuccessfulResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
            response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return View("NoAccess");
        }

        var errorModel = new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier
        };
        return View("Error", errorModel);
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/tasks/dashboard");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var dashboardData = await response.Content.ReadFromJsonAsync<List<DashboardTaskSummaryDto>>();
        return View(dashboardData);
    }

    [HttpGet]
    public async Task<IActionResult> TasksByStatus(int statusId, int page = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/tasks/status/{statusId}?page={page}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
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
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/user?page={page}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var pagedTasks = await response.Content.ReadFromJsonAsync<PagedListViewModel<TaskViewModel>>();
        ViewData["PageSize"] = pageSize;
        return View(pagedTasks);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/details/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var task = await response.Content.ReadFromJsonAsync<TaskViewModel>();
        return View(task);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var client = _httpClientFactory.CreateClient();
        var permResponse = await client.GetAsync($"{_apiBaseUrl}/todo/cancreate");
        if (!permResponse.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(permResponse);
        }

        var response = await client.GetAsync($"{_apiBaseUrl}/todo/statuses");
        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
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
        var response = await client.PostAsync($"{_apiBaseUrl}/todo", content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return View("NoAccess");
            }
            ModelState.AddModelError("", "Failed to create task.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/edit/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var task = await response.Content.ReadFromJsonAsync<TaskViewModel>();

        var statusesResponse = await client.GetAsync($"{_apiBaseUrl}/todo/statuses");
        if (!statusesResponse.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(statusesResponse);
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
        var response = await client.PutAsync($"{_apiBaseUrl}/todo/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return View("NoAccess");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var errorModel = new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier
                };
                ModelState.AddModelError("", "Task not found or you do not have permission to edit this task.");
                return View("Error", errorModel);
            }
            ModelState.AddModelError("", "Failed to update task.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/delete/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var task = await response.Content.ReadFromJsonAsync<TaskViewModel>();
        return View(task);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{_apiBaseUrl}/todo/delete/{id}", null);

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        return RedirectToAction("Index");
    }
}
