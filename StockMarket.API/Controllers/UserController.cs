using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockMarket.API.Controllers.Services;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;
        private readonly IRoleRepository _roleRepository;
        private readonly StockService _stockService;

        public UserController(IUserRepository userRepository, TokenService tokenService, IRoleRepository roleRepository, StockService stockService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
            _stockService = stockService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserRegisterDTO user)
        {
            var newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                RoleId = 2,
                Balance = 0
            };
            var registeredUser = await _userRepository.Create(newUser);
            return CreatedAtAction(nameof(Register), new { id = registeredUser.Id }, registeredUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] UserLoginDTO user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data");
            }
            var existingUser = await _userRepository.GetByEmail(user.Email);
            if (existingUser == null || existingUser.Password != user.Password)
            {
                return Unauthorized("Invalid email or password");
            }

            var role = await _roleRepository.Get(existingUser.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role.Name)
            };
            var token = _tokenService.GenerateToken(claims);
            return Ok(new { User = existingUser, Token = token });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetUser/{id:int}")]
        public async Task<ActionResult<User?>> GetUser(int id)
        {
            return await _userRepository.Get(id);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            var newUser = await _userRepository.Create(user);
            return CreatedAtAction(nameof(CreateUser), new { id = newUser.Id }, newUser);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateUser/{id:int}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            await _userRepository.Update(user);

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var userToDelete = await _userRepository.Get(id);
            if (userToDelete == null)
                return BadRequest();
            await _userRepository.Delete(userToDelete.Id);

            return NoContent();
        }
    }
}
