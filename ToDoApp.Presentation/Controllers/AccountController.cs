using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Presentation.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var client = _httpClientFactory.CreateClient("ToDoApi");
        var response = await client.PostAsJsonAsync("/api/account/login", model);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (result?.Token != null)
            {
                HttpContext.Session.SetString("JWToken", result.Token);
                return RedirectToAction("Index", "User");
            }
        }
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var client = _httpClientFactory.CreateClient("ToDoApi");
        var response = await client.PostAsJsonAsync("/api/account/register", model);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }
        ModelState.AddModelError(string.Empty, "Failed to register user.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JWToken");
        return RedirectToAction("Login");
    }

    public class TokenResponse
    {
        public string Token { get; set; }
    }
}
