using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ILogger<PortfolioController> _logger;
        public PortfolioController(IPortfolioRepository portfolioRepository, ILogger<PortfolioController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreatePortfolio")]
        public async Task<ActionResult<Portfolio>> CreatePortfolio([FromBody] PortfolioDTO portfolio)
        {
            _logger.LogInformation("'CreatePortfolio' method executed.");
            try
            {
                var newPortfolio = new Portfolio
                {
                    UserId = portfolio.UserId,
                    StockId = portfolio.StockId
                };
                var createdPortfolio = await _portfolioRepository.Create(newPortfolio);

                _logger.LogInformation("Portfolio created successfully. PortfolioId: {PortfolioId}", createdPortfolio.Id);
                return CreatedAtAction(nameof(CreatePortfolio), new { id = createdPortfolio.Id }, createdPortfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during portfolio creation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetByUserId/{userId:int}")]
        public async Task<ActionResult<List<Portfolio>>> GetByUserId(int userId)
        {
            _logger.LogInformation("'GetByUserId' method executed.");
            try
            {
                var portfolios = await _portfolioRepository.GetByUserId(userId);

                if (portfolios == null || portfolios.Count == 0)
                {
                    _logger.LogWarning("Portfolio not found for user with ID: {UserId}", userId);
                    return NotFound($"Portfolio not found for user with ID: {userId}");
                }

                _logger.LogInformation("Portfolios retrieved successfully for user with ID: {UserId}. PortfolioCount: {PortfolioCount}", userId, portfolios.Count);
                return portfolios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting portfolios by user ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetByStockId/{stockId:int}")]
        public async Task<ActionResult<List<Portfolio>>> GetByStockId(int stockId)
        {
            _logger.LogInformation("'GetByStockId' method executed.");
            try
            {
                var portfolios = await _portfolioRepository.GetByStockId(stockId);

                if (portfolios == null || portfolios.Count == 0)
                {
                    _logger.LogWarning("Portfolio not found for stock with ID: {StockId}", stockId);
                    return NotFound($"Portfolio not found for stock with ID: {stockId}");
                }

                _logger.LogInformation("Portfolios retrieved successfully for stock with ID: {StockId}. PortfolioCount: {PortfolioCount}", stockId, portfolios.Count);
                return portfolios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting portfolios by stock ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetPortfolio/{id:int}")]
        public async Task<ActionResult<Portfolio?>> GetPortfolio(int id)
        {
            _logger.LogInformation("'GetPortfolio' method executed.");
            try
            {
                var portfolio = await _portfolioRepository.Get(id);

                if (portfolio == null)
                {
                    _logger.LogWarning("Portfolio not found. PortfolioId: {PortfolioId}", id);
                    return NotFound($"Portfolio not found. PortfolioId: {id}");
                }

                _logger.LogInformation("Portfolio retrieved successfully. PortfolioId: {PortfolioId}", id);
                return portfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting portfolio by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
