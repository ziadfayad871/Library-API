using AutoMapper;
using Library_API.Dtos;
using Library_API.Model;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library_API.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(
            IMapper mapper,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            // If user is null, it means the username is unique (not found in the database)
            return user == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByNameAsync(loginRequestDTO.Username);
            if (user == null)
            {
                return null;
            }

            var passwordIsValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (!passwordIsValid)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (string.IsNullOrEmpty(role))
            {
                return null;
            }

            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = role;

            var secret = _configuration.GetValue<string>("ApiSettings:Secret");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new LoginResponseDTO { User = userDTO, Token = tokenString };
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var existingUser = await _userManager.FindByNameAsync(registrationRequestDTO.Username);
            if (existingUser != null)
            {
                return null;
            }

            var newUser = _mapper.Map<ApplicationUser>(registrationRequestDTO);
            newUser.UserName = registrationRequestDTO.Username;
            newUser.Email = registrationRequestDTO.Username;
            newUser.Name = registrationRequestDTO.Name;

            var createResult = await _userManager.CreateAsync(newUser, registrationRequestDTO.Password);
            if (!createResult.Succeeded)
            {
                return null;
            }

            var role = string.IsNullOrWhiteSpace(registrationRequestDTO.Role)
                ? "Customer"
                : registrationRequestDTO.Role;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);
                    return null;
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, role);
            if (!addToRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return null;
            }

            var userDTO = _mapper.Map<UserDTO>(newUser);
            userDTO.Role = role;
            return userDTO;
        }
    }
}
