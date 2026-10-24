using LibraryWeb.Models.AuthDTO;
using LibraryWeb.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LibraryWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequestDTO);
            }

            var apiResponse = await _authService.LoginAsync(loginRequestDTO);
            if (apiResponse == null || !apiResponse.IsSuccess)
            {
                TempData["ErrorMessage"] = apiResponse?.ErrorMessages?.FirstOrDefault() ?? "Invalid username or password.";
                return View(loginRequestDTO);
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(
                apiResponse.Result.ToString(), jsonOptions);

            Response.Cookies.Append("JWTToken", loginResponse.Token);

            TempData["SuccessMessage"] = "Login successful.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationRequestDTO);
            }

            var apiResponse = await _authService.RegisterAsync(registrationRequestDTO);
            if (apiResponse == null || !apiResponse.IsSuccess)
            {
                TempData["ErrorMessage"] = apiResponse?.ErrorMessages?.FirstOrDefault() ?? "Registration failed.";
                return View(registrationRequestDTO);
            }

            TempData["SuccessMessage"] = "Registration successful.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Index", "Home");
        }
    }
}
