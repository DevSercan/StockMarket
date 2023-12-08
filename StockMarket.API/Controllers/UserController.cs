using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            return Ok(existingUser);
        }

        // HttpPut: Tüm bilgileri günceller. HttpPatch: Belirli bir alanı günceller. Bu yüzden HttpPatch kullanıyoruz.
        [HttpPatch("ChangeUserRole/{userId:int}/{roleId:int}")]
        public async Task<ActionResult<User>> ChangeUserRole(int userId, int roleId)
        {
            try
            {
                await _userRepository.ChangeUserRole(userId, roleId);
                var updatedUser = await _userRepository.Get(userId);

                if (updatedUser == null)
                {
                    return NotFound("User not found after role change");
                }

                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPatch("UpdateBalance/{userId:int}/{balance:decimal}")]
        [Authorize]
        public async Task<ActionResult<User>> UpdateBalance(int userId, decimal balance)
        {
            try
            {
                await _userRepository.UpdateBalance(userId, balance);
                var updatedUser = await _userRepository.Get(userId);

                if (updatedUser == null)
                {
                    return NotFound("User not found after balance update");
                }

                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User?>> GetUser(int id)
        {
            return await _userRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            var newUser = await _userRepository.Create(user);
            return CreatedAtAction(nameof(PostUser), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            await _userRepository.Update(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
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
