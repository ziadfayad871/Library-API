using AutoMapper;
using Library_API.Data;
using Library_API.Dtos;
using Library_API.Models;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserRepository(ApplicationDbContext context, IMapper mapper , IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            var exists = await _context.Users.AnyAsync(u => u.UserName == username);

            // If user is null, it means the username is unique (not found in the database)
            return !exists;



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
