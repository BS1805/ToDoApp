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

    public async Task<IActionResult> ViewAllUsers()
    {
        var users = await _httpClient.GetFromJsonAsync<List<(ApplicationUser User, IList<string> Roles, int TaskCount)>>("/api/admin/users");
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePermissions(UpdatePermissionsRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("/api/admin/permissions", request);
        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(ViewAllUsers));

        return BadRequest("Failed to update permissions.");
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
