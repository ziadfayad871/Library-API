using AutoMapper;
using Library_API.Data;
using Library_API.Dtos;
using Library_API.Models;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library_API.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        public UserRepository(ApplicationDbContext context, IMapper mapper , IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            var exists = await _context.Users.AnyAsync(u => u.UserName == username);

            // If user is null, it means the username is unique (not found in the database)
            return !exists;



        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginRequestDTO.Username);
            if (user == null)
            {
                return null;
            }
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequestDTO.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }
        var userDTO = _mapper.Map<UserDTO>(user);
            var secret = _configuration.GetValue<string>("ApiSettings:Secret");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var Claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new Claim(ClaimTypes.Name, user.UserName),
    new Claim(ClaimTypes.Role, user.Role),
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
};
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
    claims: Claims,
    expires: DateTime.UtcNow.AddDays(7),
    signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new LoginResponseDTO { User = userDTO, Token = tokenString };
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            // Uniqueness check for username can be implemented here before saving the user to the database
            if (!await IsUniqueUser(registrationRequestDTO.Username))
            {
                return null;
            }
            var user = _mapper.Map<User>(registrationRequestDTO);
            
            await _context.Users.AddAsync(user);
            
            // password hashing can be implemented here before saving the user to the database : 
            if (registrationRequestDTO.Password != null)
            {
                user.Password = _passwordHasher.HashPassword(user, registrationRequestDTO.Password);
            }
            
            // Save the changes to the database
            await _context.SaveChangesAsync();
            return _mapper.Map<UserDTO>(user);
        }
    }
}
