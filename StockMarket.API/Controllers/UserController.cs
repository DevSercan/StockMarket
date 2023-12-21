using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMarket.API.Controllers.Services;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IRoleRepository _roleRepository;
        private readonly IStockService _stockService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ITokenService tokenService, IRoleRepository roleRepository, IStockService stockService, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
            _stockService = stockService;
            _logger = logger;
        }

        private User MapToUser(UserRegisterDTO user)
        {
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                RoleId = 2,
                Balance = 0
            };
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody] UserRegisterDTO user)
        {
            _logger.LogInformation("'Register' method executed.");
            try
            {
                var existingUser = await _userRepository.GetByEmail(user.Email);
                if (existingUser != null)
                {
                    _logger.LogInformation("User registration failed. Email '{Email}' is already taken.", user.Email);
                    return StatusCode(StatusCodes.Status400BadRequest, "Email is already taken");
                }
                var newUser = MapToUser(user);
                var registeredUser = await _userRepository.Create(newUser);
                _logger.LogInformation("User registered successfully. UserId: {UserId}", registeredUser.Id);
                return CreatedAtAction(nameof(Register), new { id = registeredUser.Id }, registeredUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] UserLoginDTO user)
        {
            _logger.LogInformation("'Login' method executed.");
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("Invalid user data during login attempt.");
                    return BadRequest("Invalid user data");
                }
                var existingUser = await _userRepository.GetByEmail(user.Email);
                if (existingUser == null || existingUser.Password != user.Password)
                {
                    _logger.LogWarning("Invalid email or password during login attempt for email: {UserEmail}", user.Email);
                    return Unauthorized("Invalid email or password");
                }

                var claims = await _tokenService.GenerateClaims(existingUser);
                var token = _tokenService.GenerateToken(claims);

                _logger.LogInformation("User logged in successfully. UserId: {UserId}", existingUser.Id);
                return Ok(new { User = existingUser, Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetUser/{id:int}")]
        public async Task<ActionResult<User?>> GetUser(int id)
        {
            _logger.LogInformation("'GetUser' method executed.");
            try
            {
                var user = await _userRepository.Get(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found with id: {UserId}", id);
                    return NotFound();
                }
                _logger.LogInformation("User found with id: {UserId}", id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("'DeleteUser' method executed.");
            try
            {
                var userToDelete = await _userRepository.Get(id);
                if (userToDelete == null)
                {
                    _logger.LogWarning("User not found with id: {UserId}", id);
                    return BadRequest();
                }
                await _userRepository.Delete(userToDelete.Id);

                _logger.LogInformation("User deleted successfully. UserId: {UserId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user deletion.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
