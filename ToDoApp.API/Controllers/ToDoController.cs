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

        /// <summary>
        /// Gets dashboard summary data for the current user.
        /// </summary>
        [HttpGet("tasks/dashboard")]
        [ProducesResponseType(typeof(List<DashboardTaskSummaryDto>), 200)]
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetUserId();
            var dashboardData = await _toDoService.GetDashboardDataAsync(userId);
            return Ok(dashboardData);
        }

        /// <summary>
        /// Gets a paged list of tasks by status for the current user.
        /// </summary>
        [HttpGet("tasks/status/{statusId}")]
        [ProducesResponseType(typeof(PagedListViewModel<TaskViewModel>), 200)]
        public async Task<IActionResult> TasksByStatus(int statusId, int page = 1, int pageSize = 10)
        {
            var userId = GetUserId();
            var pagedTasks = await _toDoService.GetPagedTasksByStatusAsync(userId, statusId, page, pageSize);
            return Ok(pagedTasks);
        }

        /// <summary>
        /// Gets a paged list of all tasks for the current user.
        /// </summary>
        [HttpGet("user")]
        [ProducesResponseType(typeof(PagedListViewModel<TaskViewModel>), 200)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var userId = GetUserId();
            var pagedTasks = await _toDoService.GetPagedToDoItemsAsync(userId, page, pageSize);
            return Ok(pagedTasks);
        }

        /// <summary>
        /// Gets the details of a specific task for the current user.
        /// </summary>
        [HttpGet("details/{id}")]
        [ProducesResponseType(typeof(TaskViewModel), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDetails(int id)
        {
            if ((GetUserPermissions() & (int)UserPermission.Details) == 0)
                return Forbid();

            var userId = GetUserId();
            var task = await _toDoService.GetToDoItemForUser(id, userId);
            if (task == null)
                return NotFound();
            return Ok(task);
        }

        /// <summary>
        /// Gets a task for editing for the current user.
        /// </summary>
        [HttpGet("edit/{id}")]
        [ProducesResponseType(typeof(TaskViewModel), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEdit(int id)
        {
            if ((GetUserPermissions() & (int)UserPermission.Edit) == 0)
                return Forbid();

            var userId = GetUserId();
            var task = await _toDoService.GetToDoItemForUser(id, userId);
            if (task == null)
                return NotFound();
            return Ok(task);
        }

        /// <summary>
        /// Checks if the current user can create a task.
        /// </summary>
        [HttpGet("cancreate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult CanCreate()
        {
            if ((GetUserPermissions() & (int)UserPermission.Create) == 0)
                return Forbid();
            return Ok();
        }

        /// <summary>
        /// Creates a new task for the current user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TaskViewModel), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create([FromBody] TaskViewModel model)
        {
            if ((GetUserPermissions() & (int)UserPermission.Create) == 0)
                return Forbid();

            var userId = GetUserId();
            var created = await _toDoService.CreateToDoItem(model, userId);
            return Ok(created);
        }

        /// <summary>
        /// Updates an existing task for the current user.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TaskViewModel), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Edit(int id, [FromBody] TaskViewModel model)
        {
            if ((GetUserPermissions() & (int)UserPermission.Edit) == 0)
                return Forbid();

            var userId = GetUserId();
            var updated = await _toDoService.UpdateToDoItem(model, userId);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Gets a task for deletion for the current user.
        /// </summary>
        [HttpGet("delete/{id}")]
        [ProducesResponseType(typeof(TaskViewModel), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDelete(int id)
        {
            if ((GetUserPermissions() & (int)UserPermission.Delete) == 0)
                return Forbid();

            var userId = GetUserId();
            var task = await _toDoService.GetToDoItemForUser(id, userId);
            if (task == null)
                return NotFound();
            return Ok(task);
        }

        /// <summary>
        /// Confirms deletion of a task for the current user.
        /// </summary>
        [HttpPost("delete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if ((GetUserPermissions() & (int)UserPermission.Delete) == 0)
                return Forbid();

            var userId = GetUserId();
            var deleted = await _toDoService.DeleteToDoItemForUser(id, userId);
            if (deleted is bool b && !b)
                return Forbid();
            return NoContent();
        }

        /// <summary>
        /// Gets the list of available statuses.
        /// </summary>
        [HttpGet("statuses")]
        [ProducesResponseType(typeof(IEnumerable<Status>), 200)]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _toDoService.GetStatusesAsync();
            return Ok(statuses);
        }

        // Helper methods
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
    }
}
