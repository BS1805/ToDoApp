using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ToDoApp.FrontEnd.Models;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;
    private readonly string _apiKey;

    public AccountController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = apiSettings.Value.BaseUrl.TrimEnd('/');
        _apiKey = apiSettings.Value.ApiKey;
    }

    private HttpClient CreateClientWithApiKey()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        return client;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = CreateClientWithApiKey();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{_apiBaseUrl}/account/login", content);

        if (response.IsSuccessStatusCode)
        {
            // Retrieve the logged-in user's roles
            var userResponse = await client.GetAsync($"{_apiBaseUrl}/account/getroles");
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

        var client = CreateClientWithApiKey();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{_apiBaseUrl}/account/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Admin");
        }

        ModelState.AddModelError("", "Registration failed.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var client = CreateClientWithApiKey();
        var response = await client.PostAsync($"{_apiBaseUrl}/account/logout", null);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Logout failed.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var client = CreateClientWithApiKey();
        var response = await client.GetAsync($"{_apiBaseUrl}/account/getroles");

        if (response.IsSuccessStatusCode)
        {
            var roles = await response.Content.ReadFromJsonAsync<List<string>>();
            return Json(roles);
        }

        return Unauthorized();
    }
}
