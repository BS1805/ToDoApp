using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ToDoController(IToDoService toDoService, UserManager<ApplicationUser> userManager)
        {
            _toDoService = toDoService;
            _userManager = userManager;
        }

        private int GetUserPermissions()
        {
            var permissionsClaim = User.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsClaim)) return 0;
            return int.TryParse(permissionsClaim, out var p) ? p : 0;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("tasks/dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            // No permission check for dashboard, add if needed
            var userId = GetUserId();
            var dashboardData = await _toDoService.GetDashboardDataAsync(userId);
            return Ok(dashboardData);
        }

        [HttpGet("tasks/status/{statusId}")]
        public async Task<IActionResult> TasksByStatus(int statusId, int page = 1, int pageSize = 10)
        {
            // No permission check for listing, add if needed
            var userId = GetUserId();
            var pagedTasks = await _toDoService.GetPagedTasksByStatusAsync(userId, statusId, page, pageSize);
            return Ok(pagedTasks);
        }

        [HttpGet("user")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // No permission check for listing, add if needed
            var userId = GetUserId();
            var pagedTasks = await _toDoService.GetPagedToDoItemsAsync(userId, page, pageSize);
            return Ok(pagedTasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if ((GetUserPermissions() & (int)UserPermission.Details) == 0)
                return Forbid();

            var userId = GetUserId();
            var task = await _toDoService.GetToDoItemForUser(id, userId);
            if (task == null)
                return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskViewModel model)
        {
            if ((GetUserPermissions() & (int)UserPermission.Create) == 0)
                return Forbid();

            var userId = GetUserId();
            var created = await _toDoService.CreateToDoItem(model, userId);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] TaskViewModel model)
        {
            if ((GetUserPermissions() & (int)UserPermission.Edit) == 0)
                return Forbid();

            var userId = GetUserId();
            var updated = await _toDoService.UpdateToDoItem(model, userId);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if ((GetUserPermissions() & (int)UserPermission.Delete) == 0)
                return Forbid();

            var userId = GetUserId();
            await _toDoService.DeleteToDoItemForUser(id, userId);
            return NoContent();
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _toDoService.GetStatusesAsync();
            return Ok(statuses);
        }
    }
}
