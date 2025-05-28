using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Linq;

namespace ToDoApp.API.Controllers
{
    /// <summary>
    /// Provides endpoints for user authentication, registration, role retrieval, permissions, and logout.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Authenticates a user and signs them in.
        /// </summary>
        /// <param name="model">The login credentials.</param>
        /// <returns>200 OK if successful, 401 Unauthorized if credentials are invalid.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        public async Task<object> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { Message = "Invalid login attempt." });

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
                return Unauthorized(new { Message = "Invalid login attempt." });

            // Remove existing claims and add the Permissions claim
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            var existingClaim = identity.FindFirst("Permissions");
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);
            identity.AddClaim(new Claim("Permissions", ((int)user.Permissions).ToString()));

            await _signInManager.Context.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity)
            );

            return Ok(new { Message = "Login successful." });
        }

        /// <summary>
        /// Gets the roles assigned to the currently authenticated user.
        /// </summary>
        /// <returns>List of roles for the user.</returns>
        [HttpGet("getroles")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        public async Task<object> GetRoles()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new { Message = "Unauthorized" });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="model">The registration details.</param>
        /// <returns>200 OK if successful, 400 Bad Request if registration fails.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<object> Register([FromBody] RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { Message = "Registration successful." });
            }
            // Return all errors as a single string or as a list
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Message = "Failed to register user.", Errors = errors });
        }

        /// <summary>
        /// Gets the permissions assigned to the currently authenticated user.
        /// </summary>
        /// <returns>The permissions value as an integer.</returns>
        [HttpGet("permissions")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public object GetPermissions()
        {
            var permissionsClaim = User.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsClaim))
            {
                return Ok(new { Permissions = 0 }); // No permissions
            }

            if (int.TryParse(permissionsClaim, out var permissions))
            {
                return Ok(new { Permissions = permissions });
            }

            return BadRequest(new { Message = "Invalid permissions format." });
        }

        /// <summary>
        /// Signs out the currently authenticated user.
        /// </summary>
        /// <returns>200 OK if successful.</returns>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<object> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful." });
        }
    }
}
