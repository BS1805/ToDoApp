using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ToDoApp.FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;


public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:44369/api/admin/users");

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
        var response = await client.PostAsync($"https://localhost:44369/api/admin/users/{userId}/activate", null);

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
        var response = await client.PostAsync($"https://localhost:44369/api/admin/users/{userId}/deactivate", null);

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
        var response = await client.DeleteAsync($"https://localhost:44369/api/admin/users/{userId}");

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
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PutAsync("https://localhost:44369/api/admin/permissions", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to update permissions.");
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }
}
