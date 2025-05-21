using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ToDoApp.FrontEnd.Models;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://localhost:44369/api/account/login", content);

        if (response.IsSuccessStatusCode)
        {
            // Retrieve the logged-in user's roles
            var userResponse = await client.GetAsync("https://localhost:44369/api/account/getroles");
            if (userResponse.IsSuccessStatusCode)
            {
                var roles = await userResponse.Content.ReadFromJsonAsync<List<string>>();

                // Redirect based on role
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("Index", "ToDo");
                }
            }

            // Default redirect for non-admin users
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://localhost:44369/api/account/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Registration failed.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync("https://localhost:44369/api/account/logout", null);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Logout failed.");
        return RedirectToAction("Index", "Home...");
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:44369/api/account/getroles");

        if (response.IsSuccessStatusCode)
        {
            var roles = await response.Content.ReadFromJsonAsync<List<string>>();
            return Json(roles);
        }

        return Unauthorized();
    }
}
