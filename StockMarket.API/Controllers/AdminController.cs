using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<AdminController> _logger;
        public AdminController(ICommissionRepository commissionRepository, IUserRepository userRepository, ILogger<AdminController> logger)
        {
            _commissionRepository = commissionRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateBalance/{userId:int}/{balance:decimal}")]
        public async Task<ActionResult<User>> UpdateBalance(int userId, decimal balance)
        {
            _logger.LogInformation("'UpdateBalance' method executed.");
            try
            {
                await _userRepository.UpdateBalance(userId, balance);
                var updatedUser = await _userRepository.Get(userId);

                if (updatedUser == null)
                {
                    _logger.LogWarning("User not found after balance update. UserId: {UserId}", userId);
                    return NotFound("User not found after balance update");
                }

                _logger.LogInformation("User balance updated successfully. UserId: {UserId}", userId);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception occurred during balance update. UserId: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user balance update. UserId: {UserId}", userId);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // HttpPut: Updates all information. HttpPatch: Updates a specific field. That's why we use HttpPatch.
        [HttpPatch("ChangeUserRole/{userId:int}/{roleId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> ChangeUserRole(int userId, int roleId)
        {
            _logger.LogInformation("'ChangeUserRole' method executed.");
            try
            {
                await _userRepository.ChangeUserRole(userId, roleId);
                var updatedUser = await _userRepository.Get(userId);

                if (updatedUser == null)
                {
                    _logger.LogWarning("User not found after role change. UserId: {UserId}", userId);
                    return NotFound("User not found after role change");
                }

                _logger.LogInformation("User role changed successfully. UserId: {UserId}, NewRoleId: {RoleId}", userId, roleId);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception occurred during role change. UserId: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user role change. UserId: {UserId}", userId);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateCommissionRate/{id:int}/{commissionRate:decimal}")]
        public async Task<ActionResult<Commission>> UpdateCommissionRate(int id, decimal commissionRate)
        {
            _logger.LogInformation("'UpdateCommissionRate' method executed.");
            try
            {
                await _commissionRepository.UpdateCommissionRate(id, commissionRate);
                var updatedCommission = await _commissionRepository.Get(id);

                if (updatedCommission == null)
                {
                    _logger.LogWarning("Commission not found after commission rate update. CommissionId: {CommissionId}", id);
                    return NotFound("Commission not found after commission rate update");
                }

                _logger.LogInformation("Commission rate updated successfully. CommissionId: {CommissionId}, NewRate: {CommissionRate}", id, commissionRate);
                return Ok(updatedCommission);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception occurred during commission rate update. CommissionId: {CommissionId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during commission rate update. CommissionId: {CommissionId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
