using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities; // (Assuming DTOs are shared or available)

namespace ToDoApp.Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly HttpClient _httpClient;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ToDoApi");
    }

    public async Task<IActionResult> Index()
    {
        var users = await _httpClient.GetFromJsonAsync<List<(ApplicationUser User, IList<string> Roles, int TaskCount)>>("/api/admin/users");
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePermissions(UpdatePermissionsRequest model)
    {
        var client = _httpClientFactory.CreateClient();

        // Serialize Permissions as an array of integers
        var serializedModel = new
        {
            UserId = model.UserId,
            Permissions = model.Permissions
        };

        var content = new StringContent(JsonSerializer.Serialize(serializedModel), Encoding.UTF8, "application/json");
        var response = await client.PutAsync("https://localhost:44369/api/admin/permissions", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Failed to update permissions. Error: {errorContent}");
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var response = await _httpClient.DeleteAsync($"/api/admin/users/{userId}");
        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(ViewAllUsers));

        return BadRequest("Failed to delete user.");
    }
}
