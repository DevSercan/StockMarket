using Microsoft.AspNetCore.Mvc;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly ILogger<CommissionController> _logger;
        public CommissionController(ICommissionRepository commissionRepository, ILogger<CommissionController> logger)
        {
            _commissionRepository = commissionRepository;
            _logger = logger;
        }

        [HttpGet("GetCommissions/")]
        public async Task<ActionResult<List<Commission>>> GetCommissions()
        {
            _logger.LogInformation("'GetCommissions' method executed.");
            try
            {
                var commissions = await _commissionRepository.GetAll();

                if (commissions != null)
                {
                    _logger.LogInformation("All commissions have been fetched.");
                }
                else
                {
                    _logger.LogWarning("No commissions found.");
                }
                return commissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting commissions.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
