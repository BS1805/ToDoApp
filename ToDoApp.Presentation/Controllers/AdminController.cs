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

    [HttpPut("permissions")]
    public async Task<IActionResult> UpdatePermissions([FromBody] UpdatePermissionsRequest request)
    {
        if (!Enum.TryParse<UserPermission>(request.Permissions, out var permissions))
        {
            return BadRequest("Invalid permissions value.");
        }

        var success = await _userAdminService.UpdateUserPermissionsAsync(request.UserId, permissions);
        if (!success)
            return BadRequest("Failed to update permissions.");
        return NoContent();
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
