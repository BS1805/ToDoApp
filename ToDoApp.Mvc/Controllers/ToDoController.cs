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
    private readonly string _apiKey;

    public ToDoController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = apiSettings.Value.BaseUrl.TrimEnd('/');
        _apiKey = apiSettings.Value.ApiKey;
    }

    private HttpClient CreateClientWithApiKey()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        return client;
    }

    private async Task PopulateStatusesAsync()
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/statuses");
        if (response.IsSuccessStatusCode)
        {
            var statuses = await response.Content.ReadFromJsonAsync<List<Status>>();
            ViewBag.Statuses = statuses ?? new List<Status>();
        }
        else
        {
            ViewBag.Statuses = new List<Status>();
        }
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
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/tasks/dashboard");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var dashboardDataObj = await response.Content.ReadFromJsonAsync<object>();
        var dashboardData = JsonSerializer.Deserialize<List<DashboardTaskSummaryDto>>(dashboardDataObj?.ToString() ?? "[]");
        return View(dashboardData);
    }

    [HttpGet]
    public async Task<IActionResult> TasksByStatus(int statusId, int page = 1, int pageSize = 10)
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/tasks/status/{statusId}?page={page}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var pagedTasksObj = await response.Content.ReadFromJsonAsync<object>();
        var pagedTasks = JsonSerializer.Deserialize<PagedListViewModel<TaskViewModel>>(pagedTasksObj?.ToString() ?? "{}");

        // Ensure pagedTasks and Items are not null
        if (pagedTasks == null)
        {
            pagedTasks = new PagedListViewModel<TaskViewModel>
            {
                Items = new List<TaskViewModel>(),
                PageIndex = page,
                PageSize = pageSize,
                TotalPages = 0,
                TotalCount = 0
            };
        }
        else if (pagedTasks.Items == null)
        {
            pagedTasks.Items = new List<TaskViewModel>();
        }

        ViewData["StatusId"] = statusId;
        ViewData["PageSize"] = pageSize;
        return View(pagedTasks);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/user?page={page}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var pagedTasks = await response.Content.ReadFromJsonAsync<PagedListViewModel<TaskViewModel>>();

        if (pagedTasks == null)
        {
            pagedTasks = new PagedListViewModel<TaskViewModel>
            {
                Items = new List<TaskViewModel>(),
                PageIndex = page,
                PageSize = pageSize,
                TotalPages = 0,
                TotalCount = 0
            };
        }
        else if (pagedTasks.Items == null)
        {
            pagedTasks.Items = new List<TaskViewModel>();
        }

        ViewData["PageSize"] = pageSize;
        return View(pagedTasks);
    }



    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/details/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var taskObj = await response.Content.ReadFromJsonAsync<object>();
        var task = JsonSerializer.Deserialize<TaskViewModel>(taskObj?.ToString() ?? "{}");
        return View(task);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var client = CreateClientWithApiKey();
        var permResponse = await client.GetAsync($"{_apiBaseUrl}/todo/cancreate");
        if (!permResponse.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(permResponse);
        }

        await PopulateStatusesAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateStatusesAsync();
            return View(model);
        }

        var client = CreateClientWithApiKey();
        var content = JsonContent.Create(model);
        var response = await client.PostAsync($"{_apiBaseUrl}/todo", content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return View("NoAccess");
            }
            ModelState.AddModelError("", "Failed to create task.");
            await PopulateStatusesAsync();
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/edit/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var taskObj = await response.Content.ReadFromJsonAsync<object>();
        var task = JsonSerializer.Deserialize<TaskViewModel>(taskObj?.ToString() ?? "{}");

        await PopulateStatusesAsync();
        return View(task);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateStatusesAsync();
            return View(model);
        }

        var client = CreateClientWithApiKey();
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
            await PopulateStatusesAsync();
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/todo/delete/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        var taskObj = await response.Content.ReadFromJsonAsync<object>();
        var task = JsonSerializer.Deserialize<TaskViewModel>(taskObj?.ToString() ?? "{}");
        return View(task);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = CreateClientWithApiKey();
        var response = await client.PostAsync($"{_apiBaseUrl}/todo/delete/{id}", null);

        if (!response.IsSuccessStatusCode)
        {
            return HandleUnsuccessfulResponse(response);
        }

        return RedirectToAction("Index");
    }
}
