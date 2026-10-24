using LibraryWeb.Models.AuthDTO;
using LibraryWeb.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            // Deserialize the API response to get the user information and token
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(
                JsonSerializer.Serialize(apiResponse.Result), jsonOptions);
            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginResponse.User.UserName),
                new Claim(ClaimTypes.Role, loginResponse.User.Role)
            };
            // Create a claims identity and sign in the user
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            // Store the JWT token in a secure cookie
            Response.Cookies.Append("JWTToken", loginResponse.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            // Optionally, you can store the token in TempData or ViewData for use in the next request
            TempData["SuccessMessage"] = "Login successful.";
            // Redirect to the home page or any other page after successful login
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Index", "Home");
        }
    }
}
