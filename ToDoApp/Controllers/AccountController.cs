using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Models;
using ToDoApp.Models.ViewModels;

namespace ToDoApp.Controllers;


public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                ModelState.AddModelError(string.Empty, "An error occurred while registering the user.");
            }
        }
        return View(model);
    }

    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                ModelState.AddModelError(string.Empty, "An error occurred while logging in.");
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while logging out.");

            throw new Exception("An error occurred while logging out.", ex);
        }
        return RedirectToAction("Index", "Home");
    }
}
