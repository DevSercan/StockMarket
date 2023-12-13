using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommissionRepository _commissionRepository;
        public AdminController(ICommissionRepository commissionRepository, IUserRepository userRepository)
        {
            _commissionRepository = commissionRepository;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateBalance/{userId:int}/{balance:decimal}")]
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

        // HttpPut: Tüm bilgileri günceller. HttpPatch: Belirli bir alanı günceller. Bu yüzden HttpPatch kullanıyoruz.
        [HttpPatch("ChangeUserRole/{userId:int}/{roleId:int}")]
        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateCommissionRate/{id:int}/{commissionRate:decimal}")]
        public async Task<ActionResult<Commission>> UpdateCommissionRate(int id, decimal commissionRate)
        {
            try
            {
                await _commissionRepository.UpdateCommissionRate(id, commissionRate);
                var updatedCommission = await _commissionRepository.Get(id);
                if (updatedCommission == null)
                {
                    return NotFound("Commission not found after commission rate update");
                }
                return Ok(updatedCommission);
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
    }
}
