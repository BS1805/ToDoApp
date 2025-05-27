using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ToDoApp.FrontEnd.Models;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;

    public AdminController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = apiSettings.Value.BaseUrl.TrimEnd('/');
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/admin/users");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to load user data.");
            return View("Error");
        }

        var users = await response.Content.ReadFromJsonAsync<List<AdminUserDto>>();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{_apiBaseUrl}/admin/users/{userId}/activate", null);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to activate user.");
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{_apiBaseUrl}/admin/users/{userId}/deactivate", null);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to deactivate user.");
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.DeleteAsync($"{_apiBaseUrl}/admin/users/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to delete user.");
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePermissions(UpdatePermissionsRequest model)
    {
        var client = _httpClientFactory.CreateClient();

        var serializedModel = new
        {
            UserId = model.UserId,
            Permissions = model.Permissions
        };

        var content = new StringContent(JsonSerializer.Serialize(serializedModel), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{_apiBaseUrl}/admin/permissions", content);

        return RedirectToAction("Index");
    }
}
