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
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        public PortfolioController(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreatePortfolio")]
        public async Task<ActionResult<Portfolio>> CreatePortfolio([FromBody] PortfolioDTO portfolio)
        {
            var newPortfolio = new Portfolio
            {
                UserId = portfolio.UserId,
                StockId = portfolio.StockId
            };
            var createdPortfolio = await _portfolioRepository.Create(newPortfolio);
            return CreatedAtAction(nameof(CreatePortfolio), new { id = createdPortfolio.Id }, createdPortfolio);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetByUserId/{userId:int}")]
        public async Task<ActionResult<List<Portfolio>>> GetByUserId(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserId(userId);
            if (portfolios == null || portfolios.Count == 0)
            {
                return NotFound($"Portfolio not found for user with ID: {userId}");
            }
            return portfolios;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetByStockId/{stockId:int}")]
        public async Task<ActionResult<List<Portfolio>>> GetByStockId(int stockId)
        {
            var portfolios = await _portfolioRepository.GetByStockId(stockId);
            if (portfolios == null || portfolios.Count == 0)
            {
                return NotFound($"Portfolio not found for stock with ID: {stockId}");
            }
            return portfolios;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetPortfolio/{id:int}")]
        public async Task<ActionResult<Portfolio?>> GetPortfolio(int id)
        {
            return await _portfolioRepository.Get(id);
        }
    }
}
