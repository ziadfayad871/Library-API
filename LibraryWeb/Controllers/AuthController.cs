using LibraryWeb.Models.AuthDTO;
using LibraryWeb.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

            if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                TempData["ErrorMessage"] = "Login failed: the server did not return a valid token.";
                return View(loginRequestDTO);
            }

            List<Claim> claims;
            try
            {
                claims = ReadClaimsFromToken(loginResponse.Token);
            }
            catch (SecurityTokenExpiredException)
            {
                TempData["ErrorMessage"] = "Login failed: the token has expired.";
                return View(loginRequestDTO);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Login failed: the token could not be read.";
                return View(loginRequestDTO);
            }

            if (claims == null)
            {
                TempData["ErrorMessage"] = "Login failed: the token is missing required claims.";
                return View(loginRequestDTO);
            }

            // Create a claims identity and sign in the user
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Store the JWT token in a secure cookie
            Response.Cookies.Append("JWTToken", loginResponse.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            TempData["SuccessMessage"] = "Login successful.";
            return RedirectToAction("Index", "Home");
        }

        private static List<Claim> ReadClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            if (jwtToken.ValidTo != DateTime.MinValue && jwtToken.ValidTo < DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException("The token has expired.");
            }

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                ?? jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var displayName = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;

            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(role))
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role)
            };

            if (!string.IsNullOrWhiteSpace(email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Name, displayName));
            }

            return claims;
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
