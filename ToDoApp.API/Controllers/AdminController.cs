using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Enums;

namespace ToDoApp.API.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for managing users, their activation status, roles, and permissions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserAdminService _userAdminService;

        public AdminController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        /// <summary>
        /// Retrieves all users with their details, roles, and permissions.
        /// </summary>
        /// <returns>List of users with administrative details.</returns>
        [HttpGet("users")]
        [ProducesResponseType(typeof(List<AdminUserDto>), 200)]
        public async Task<IActionResult> Index()
        {
            var userDtos = await _userAdminService.GetAllUsersWithDetailsAsync();
            return Ok(userDtos);
        }

        /// <summary>
        /// Activates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to activate.</param>
        /// <returns>No content if successful.</returns>
        [HttpPost("users/{userId}/activate")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var success = await _userAdminService.ActivateUserAsync(userId);
            if (!success) return BadRequest("Failed to activate user.");
            return NoContent();
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate.</param>
        /// <returns>No content if successful.</returns>
        [HttpPost("users/{userId}/deactivate")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var success = await _userAdminService.DeactivateUserAsync(userId);
            if (!success) return BadRequest("Failed to deactivate user.");
            return NoContent();
        }

        /// <summary>
        /// Updates the permissions for a user.
        /// </summary>
        /// <param name="request">The permissions update request.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("permissions")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdatePermissions([FromBody] UpdatePermissionsRequest request)
        {
            var combinedPermissions = request.Permissions?.Aggregate(0, (current, permission) => current | permission) ?? 0;
            var success = await _userAdminService.UpdateUserPermissionsAsync(request.UserId, (UserPermission)combinedPermissions);
            if (!success)
                return BadRequest("Failed to update permissions.");
            return NoContent();
        }

        /// <summary>
        /// Deletes a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("users/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var success = await _userAdminService.DeleteUserAsync(userId);
            if (!success)
                return BadRequest("Failed to delete user.");
            return NoContent();
        }
    }
}
