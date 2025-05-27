using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid login attempt.");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid login attempt.");

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

            return Ok();
        }

        /// <summary>
        /// Gets the roles assigned to the currently authenticated user.
        /// </summary>
        /// <returns>List of roles for the user.</returns>
        [HttpGet("getroles")]
        [Authorize]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetRoles()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="model">The registration details.</param>
        /// <returns>200 OK if successful, 400 Bad Request if registration fails.</returns>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return Ok();
            }
            return BadRequest("Failed to register user.");
        }

        /// <summary>
        /// Gets the permissions assigned to the currently authenticated user.
        /// </summary>
        /// <returns>The permissions value as an integer.</returns>
        [HttpGet("permissions")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetPermissions()
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

            return BadRequest("Invalid permissions format.");
        }

        /// <summary>
        /// Signs out the currently authenticated user.
        /// </summary>
        /// <returns>200 OK if successful.</returns>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
