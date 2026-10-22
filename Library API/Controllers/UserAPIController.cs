using Library_API.Dtos;
using Library_API.Dtos.APIResponse;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/UserAPI")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserAPIController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        // frombody
        public async Task<IActionResult> Register( [FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userDto = await _userRepository.Register(registrationRequestDTO);
            if (userDto == null)
            {
                // RETURNING A BAD REQUEST RESPONSE WITH A CUSTOM ERROR MESSAGE
                return BadRequest(new APIResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Username already exists." }
                });
            }
            return Ok(new APIResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Result = userDto
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userDto = await _userRepository.Login(loginRequestDTO);
            if (userDto == null)
            {
                return Unauthorized(new APIResponse
                {
                    StatusCode = 401,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Username or password is incorrect." }
                });
            }
            return Ok(new APIResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Result = userDto
            });
        }
    }
}
